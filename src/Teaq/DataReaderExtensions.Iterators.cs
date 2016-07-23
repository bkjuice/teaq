﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Reflection;
using Teaq.Configuration;
using Teaq.FastReflection;

namespace Teaq
{
    public static partial class DataReaderExtensions
    {
        private sealed class CustomHandlerIterator<T> : IEnumerator<T>, IEnumerable<T>
        {
            private readonly IDataReader reader;

            private readonly Action onCompleteCallback;

            private readonly IDataHandler<T> handler;

            public CustomHandlerIterator(IDataReader reader, IDataHandler<T> handler, Action onCompleteCallback)
            {
                Contract.Requires(handler != null);

                handler.OnBeforeReading(reader);
                this.onCompleteCallback = onCompleteCallback;
                this.handler = handler;
                this.reader = reader;
            }

            public T Current
            {
                get
                {
                    return this.handler.ReadEntity(this.reader);
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            public void Dispose()
            {
                this.handler.OnAfterReading();
                this.onCompleteCallback?.Invoke();
            }

            public bool MoveNext()
            {
                return this.reader.Read();
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }

            public IEnumerator<T> GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this;
            }

            [ContractInvariantMethod]
            private void ObjectInvariant()
            {
                Contract.Invariant(this.handler != null);
                Contract.Invariant(this.reader != null);
            }
        }

        private sealed class SimpleTypeIterator<T> : IEnumerator<T>, IEnumerable<T>
        {
            private static readonly Type targetType = typeof(T);

            private static RuntimeTypeHandle targetHandle = targetType.TypeHandle;

            private readonly IDataReader reader;

            private readonly Action onCompleteCallback;

            private Func<object, object> converter;

            private NullPolicyKind nullPolicy;

            private T current;

            public SimpleTypeIterator(IDataReader reader, NullPolicyKind nullPolicy, Action onCompleteCallback)
            {
                Contract.Requires(reader != null);

                // TODO: require at least 1 field.
                this.onCompleteCallback = onCompleteCallback;
                this.nullPolicy = nullPolicy;
                var t = reader.GetFieldType(0).TypeHandle;

                this.reader = reader;
            }

            public T Current
            {
                get
                {
                    return this.current;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            public void Dispose()
            {
                this.onCompleteCallback?.Invoke();
            }

            public bool MoveNext()
            {
                if (!this.reader.Read())
                {
                    return false;
                }

                var value = reader[0];
                if (value.IsNullOrDbNull())
                {
                    if (nullPolicy == NullPolicyKind.IncludeAsDefaultValue)
                    {
                        this.current = default(T);
                        return true;
                    }

                    return this.MoveNext();
                }

                // Have to check the actual value because of underlying DBNull or "object" type in case of null value.
                if (this.converter == null)
                {
                    var t = value.GetType().TypeHandle;
                    if (t.Equals(targetHandle))
                    {
                        this.converter = o => o;
                    }
                    else
                    {
                        this.converter = GetConverter(t, targetHandle, targetType);
                    }
                }

                this.current = (T)converter(value);
                return true;
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }

            public IEnumerator<T> GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this;
            }

            [ContractInvariantMethod]
            private void ObjectInvariant()
            {
                Contract.Invariant(this.reader != null);
            }
        }

        private sealed class ComplexTypeIterator<T> : IEnumerator<T>, IEnumerable<T>
        {
            private readonly TypeDescription description;

            private readonly IDataReader reader;

            private readonly Action onCompleteCallback;

            private readonly object[] values;

            private readonly DataFieldSetter[] index;

            public ComplexTypeIterator(IDataReader reader, IDataModel model, Action onCompleteCallback)
            {
                Contract.Requires(reader != null);
                Contract.Requires(reader.FieldCount >= 0);
                Contract.Ensures(this.description != null);
                Contract.Ensures(this.values != null);
                Contract.Ensures(this.index != null);
                Contract.Ensures(this.values.Length == this.index.Length);

                this.onCompleteCallback = onCompleteCallback;
                var t = model.EnsureConcreteType<T>();
                var d = t.GetTypeDescription(MemberTypes.Constructor | MemberTypes.Property);
                var count = reader.FieldCount;
                this.index = MapFields(reader, d, t, model, count);
                this.values = new object[count];
                this.reader = reader;
                this.description = d;
            }

            public T Current
            {
                get
                {
                    // Typically one would do this in move-next, however profiling proves this to be the fastest 
                    // approach, by a significant amount of time (-~70%).
                    var entity = description.CreateInstance();
                    reader.GetValues(this.values);
                    for (int i = 0; i < this.values.Length; i++)
                    {
                        var rowMap = index[i];
                        if (rowMap.Property != null)
                        {
                            var value = values[i];
                            if (!value.IsNullOrDbNull())
                            {
                                try
                                {
                                    rowMap.SetValue(entity, value);
                                }
                                catch (InvalidCastException e)
                                {
                                    ThrowAssignmentException(reader.GetName(i), entity.GetType().Name, rowMap.Property.MemberName, e);
                                }
                            }
                        }
                    }

                    return (T)entity;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.Current;
                }
            }

            public void Dispose()
            {
                this.onCompleteCallback?.Invoke();
            }

            public bool MoveNext()
            {
                return this.reader.Read();
            }

            public void Reset()
            {
                throw new NotSupportedException();
            }

            public IEnumerator<T> GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this;
            }

            private static DataFieldSetter[] MapFields(IDataReader r, TypeDescription d, Type t, IDataModel m, int count)
            {
                Contract.Requires(r != null);
                Contract.Requires(d != null);
                Contract.Requires(t != null);
                Contract.Requires(count >= 0);
                Contract.Ensures(Contract.Result<DataFieldSetter[]>() != null);
                Contract.Ensures(Contract.Result<DataFieldSetter[]>().Length == count);

                var config = m?.GetEntityConfig(t);
                var index = new DataFieldSetter[count];
                for (int i = 0; i < index.Length; ++i)
                {
                    var name = r.GetName(i);
                    if (config != null)
                    {
                        name = config.PropertyMapping(name);
                    }
                    var property = d.GetProperty(name);
                    if (property != null)
                    {
                        var actualHandle = r.GetFieldType(i).TypeHandle;
                        var expectedHandle = property.PropertyTypeHandle;
                        if (actualHandle.Equals(expectedHandle))
                        {
                            index[i] = new DataFieldSetter(property);
                        }
                        else
                        {
                            var converter = GetConverter(actualHandle, expectedHandle, property.PropertyType);
                            index[i] = new DataFieldSetter(property, converter);
                        }
                    }
                }

                return index;
            }

            [ContractInvariantMethod]
            private void ObjectInvariant()
            {
                Contract.Invariant(this.reader != null);
                Contract.Invariant(this.description != null);
                Contract.Invariant(this.values != null);
                Contract.Invariant(this.index != null);
                Contract.Invariant(this.index.Length >= 0);
                Contract.Invariant(this.values.Length == this.index.Length);
            }
        }
    }
}