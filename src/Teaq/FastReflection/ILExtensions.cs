using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Teaq.FastReflection
{
    internal static class ILExtensions
    {
        private const bool SkipJitVerifications = true;

        private static readonly OpCode[] OpCodesvLdcI4 = new OpCode[9]
        {
            OpCodes.Ldc_I4_0,
            OpCodes.Ldc_I4_1,
            OpCodes.Ldc_I4_2,
            OpCodes.Ldc_I4_3,
            OpCodes.Ldc_I4_4,
            OpCodes.Ldc_I4_5,
            OpCodes.Ldc_I4_6,
            OpCodes.Ldc_I4_7,
            OpCodes.Ldc_I4_8
        };

        private static readonly OpCode[] OpcodesvLdArg = new OpCode[4]
        {
            OpCodes.Ldarg_0,
            OpCodes.Ldarg_1,
            OpCodes.Ldarg_2,
            OpCodes.Ldarg_3,
        };

        public static Func<Array, int, object> ReflectArrayGetValue(this Type elementType)
        {
            Contract.Requires(elementType != null);

            var method = BuildMethod(typeof(Array), new[] { typeof(Array), typeof(int) }, typeof(object));
            ILGenerator generator = method.GetILGenerator();
            generator
                .EmitIL(il => il.Emit(OpCodes.Ldarg_0))
                .EmitIL(il => il.Emit(OpCodes.Ldarg_1))
                .EmitIL(il => il.Emit(OpCodes.Ldelem, elementType))
                .EmitIL(
                    condition: elementType.IsValueType,
                    emitTo: il =>
                    {
                        il.Emit(OpCodes.Box, elementType);
                    })
                .EmitIL(il => il.Emit(OpCodes.Ret));

            return (Func<Array, int, object>)method.CreateDelegate(typeof(Func<Array, int, object>));
        }

        public static Action<Array, int, object> ReflectArraySetValue(this Type elementType)
        {
            Contract.Requires(elementType != null);

            var method = BuildMethod(typeof(Array), new[] { typeof(Array), typeof(int), typeof(object) }, typeof(void));
            ILGenerator generator = method.GetILGenerator();
            generator
                .EmitIL(il => il.Emit(OpCodes.Ldarg_0))
                .EmitIL(il => il.Emit(OpCodes.Ldarg_1))
                .EmitIL(
                    condition: elementType.IsValueType,
                    emitTo: il =>
                    {
                        il.Emit(OpCodes.Ldarg_2);
                        il.Emit(OpCodes.Unbox_Any, elementType);
                    })
                .EmitIL(
                    condition: !elementType.IsValueType,
                    emitTo: il =>
                    {
                        il.Emit(OpCodes.Ldarg_2);
                        il.Emit(OpCodes.Castclass, elementType);
                    })
                .EmitIL(il => il.Emit(OpCodes.Stelem, elementType))
                .EmitIL(il => il.Emit(OpCodes.Ret));

            return (Action<Array, int, object>)method.CreateDelegate(typeof(Action<Array, int, object>));
        }

        public static Func<int, object> ReflectArrayInitializer(this Type elementType)
        {
            var method = BuildMethod(typeof(Array), new[] { typeof(int) }, typeof(object));
            var generator = method.GetILGenerator();
            generator
               .EmitIL(il => il.Emit(OpCodes.Ldarg_0))
               .EmitIL(il => il.Emit(OpCodes.Newarr, elementType))
               .Emit(OpCodes.Ret);

            return (Func<int, object>)method.CreateDelegate(typeof(Func<int, object>));
        }

        public static Func<object, object> ReflectGetter(this FieldInfo target)
        {
            Contract.Requires(target != null);
            Contract.Requires(target.DeclaringType != null);

            var method = BuildMethod(target.DeclaringType, new[] { typeof(object) }, typeof(object));
            method.GetILGenerator().EmitFieldGetter(target);
            return (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));
        }

        public static Action<object, object> ReflectSetter(this FieldInfo target)
        {
            Contract.Requires(target != null);
            Contract.Requires(target.DeclaringType != null);

            var method = BuildMethod(target.DeclaringType, new[] { typeof(object), typeof(object) }, typeof(void));
            method.GetILGenerator().EmitFieldSetter(target);
            return (Action<object, object>)method.CreateDelegate(typeof(Action<object, object>));
        }

        public static Func<object, object> ReflectGetter(this MethodInfo target)
        {
            Contract.Requires(target != null);

            var method = BuildMethod(target.DeclaringType, new[] { typeof(object) }, typeof(object));
            method.GetILGenerator().EmitMethod(target, false);
            return (Func<object, object>)method.CreateDelegate(typeof(Func<object, object>));
        }

        public static Func<object, object, object> ReflectIndexedGetter(this MethodInfo target)
        {
            Contract.Requires(target != null);

            var method = BuildMethod(target.DeclaringType, new[] { typeof(object), typeof(object) }, typeof(object));
            method.GetILGenerator().EmitMethod(target, false);
            return (Func<object, object, object>)method.CreateDelegate(typeof(Func<object, object, object>));
        }

        public static Action<object, object> ReflectSetter(this MethodInfo target, Type propertyType)
        {
            Contract.Requires(target != null);
            Contract.Requires(target.DeclaringType != null);
            Contract.Requires(propertyType != null);

            var method = BuildMethod(target.DeclaringType, new[] { typeof(object), typeof(object) }, typeof(void));

            method.GetILGenerator().EmitPropertySetter(target, propertyType);
            return (Action<object, object>)method.CreateDelegate(typeof(Action<object, object>));
        }

        public static Action<object, object[]> ReflectIndexedSetter(this MethodInfo target)
        {
            Contract.Requires(target != null);

            var method = BuildMethod(target.DeclaringType, new[] { typeof(object), typeof(object[]) }, typeof(void));
            method.GetILGenerator().EmitMethod(target, true);
            return (Action<object, object[]>)method.CreateDelegate(typeof(Action<object, object[]>));
        }

        public static Action<object, object[]> ReflectAction(this MethodInfo target)
        {
            Contract.Requires(target != null);

            var method = BuildMethod(target.DeclaringType, new[] { typeof(object), typeof(object[]) }, typeof(void));
            method.GetILGenerator().EmitMethod(target);
            return (Action<object, object[]>)method.CreateDelegate(typeof(Action<object, object[]>));
        }

        public static Func<object, object[], object> ReflectFunc(this MethodInfo target)
        {
            Contract.Requires(target != null);

            var method = BuildMethod(target.DeclaringType, new[] { typeof(object), typeof(object[]) }, typeof(object));
            method.GetILGenerator().EmitMethod(target);
            return (Func<object, object[], object>)method.CreateDelegate(typeof(Func<object, object[], object>));
        }

        public static Func<object[], object> ReflectConstructor(this ConstructorInfo target)
        {
            Contract.Requires(target != null);

            var method = BuildMethod(target.DeclaringType, new[] { typeof(object[]) }, typeof(object));
            method.GetILGenerator().EmitConstructor(target);
            return (Func<object[], object>)method.CreateDelegate(typeof(Func<object[], object>));
        }

        public static Func<object> ReflectValueInitializer(this Type target)
        {
            Contract.Requires(target != null);

            if (!target.IsValueType)
            {
                return null;
            }

            var method = new DynamicMethod("_" + Guid.NewGuid().ToString("X"), typeof(object), Type.EmptyTypes);
            method.GetILGenerator().EmitStructInitializer(target);
            return (Func<object>)method.CreateDelegate(typeof(Func<object>));
        }

        private static void EmitStructInitializer(this ILGenerator generator, Type target)
        {
            Contract.Requires(generator != null);
            Contract.Requires(target != null);

            var lv = generator.DeclareLocal(target);
            generator.Emit(OpCodes.Ldloca_S, lv);
            generator.Emit(OpCodes.Initobj, target);
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Box, target);
            generator.Emit(OpCodes.Ret);
        }

        private static void EmitConstructor(this ILGenerator generator, ConstructorInfo target)
        {
            Contract.Requires(generator != null);
            Contract.Requires(target != null);

            var declaringType = target.DeclaringType;
            var args = target.GetParameters();
            for (int i = 0; i < args.Length; i++)
            {
                generator.EmitLoadAndCast(args[i].ParameterType, 0, i);
            }

            generator
                .EmitIL(il => il.Emit(OpCodes.Newobj, target))
                .EmitIL(il => il.Emit(OpCodes.Box, declaringType), condition: declaringType.IsValueType)
                .Emit(OpCodes.Ret);
        }

        private static void EmitFieldGetter(this ILGenerator generator, FieldInfo target)
        {
            Contract.Requires(generator != null);
            Contract.Requires(target != null);
            Contract.Requires(target.DeclaringType != null);

            var declaringType = target.DeclaringType;

            generator.EmitIL(
                    condition: !declaringType.IsClass,
                    emitThis: il =>
                    {
                        var lv = generator.DeclareLocal(declaringType);
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Unbox_Any, declaringType);
                        il.Emit(OpCodes.Stloc_0);
                        il.Emit(OpCodes.Ldloca_S, lv);
                    },
                    elseEmit: il => il.Emit(OpCodes.Ldarg_0))
                .EmitIL(il => il.Emit(OpCodes.Ldfld, target))
                .EmitIL(il => il.Emit(OpCodes.Box, target.FieldType), condition: target.FieldType.IsValueType)
                .Emit(OpCodes.Ret);
        }

        private static void EmitFieldSetter(this ILGenerator generator, FieldInfo target)
        {
            Contract.Requires(generator != null);
            Contract.Requires(target != null);
            Contract.Requires(target.DeclaringType != null);

            var declaringType = target.DeclaringType;
            generator.EmitIL(
                    condition: declaringType.IsValueType,
                    emitThis: il =>
                        {
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Unbox, declaringType);
                        },
                    elseEmit: il =>
                        {
                            il.Emit(OpCodes.Ldarg_0);
                            il.Emit(OpCodes.Castclass, declaringType);
                        })
                .EmitIL(il => il.Emit(OpCodes.Ldarg_1))
                .EmitIL(il => il.Emit(OpCodes.Castclass, target.FieldType), target.FieldType.IsClass)
                .EmitIL(il => il.Emit(OpCodes.Unbox_Any, target.FieldType), !target.FieldType.IsClass)
                .EmitIL(il => il.Emit(OpCodes.Stfld, target))
                .Emit(OpCodes.Ret);
        }

        private static void EmitPropertySetter(this ILGenerator generator, MethodInfo target, Type propertyType)
        {
            Contract.Requires(generator != null);
            Contract.Requires(target != null);
            Contract.Requires(target.DeclaringType != null);
            Contract.Requires(propertyType != null);

            var declaringType = target.DeclaringType;
            generator.EmitIL(
                    condition: declaringType.IsValueType,
                    emitThis: il =>
                    {
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Unbox, declaringType);
                    },
                    elseEmit: il =>
                    {
                        il.Emit(OpCodes.Ldarg_0);
                        il.Emit(OpCodes.Castclass, declaringType);
                    })
                .EmitIL(il => il.Emit(OpCodes.Ldarg_1))
                .EmitIL(il => il.Emit(OpCodes.Castclass, propertyType), propertyType.IsClass)
                .EmitIL(il => il.Emit(OpCodes.Unbox_Any, propertyType), !propertyType.IsClass)
                .EmitIL(il => il.Emit(OpCodes.Callvirt, target))
                .Emit(OpCodes.Ret);
        }

        private static void EmitMethod(this ILGenerator generator, MethodInfo target, bool argumentsArePacked = true, bool returnDeclaringInstance = false)
        {
            Contract.Requires(generator != null);
            Contract.Requires(target != null);

            var declaringType = target.DeclaringType ?? typeof(object);
            var arguments = target.GetParameters();
            arguments.Validate();

            generator.EmitIL(
                    condition: declaringType.IsValueType,
                    emitThis: il =>
                        {
                            var lv = generator.DeclareLocal(declaringType);
                            il.EmitLoadAndCast(declaringType, 0);
                            il.Emit(OpCodes.Stloc_0);
                            il.Emit(OpCodes.Ldloca_S, lv);
                        },
                    elseEmit: il => il.EmitLoadAndCast(declaringType, 0));

            for (int? i = 0; i < arguments.Length; i++)
            {
                generator.EmitLoadAndCast(arguments[i.Value].ParameterType, 1, argumentsArePacked ? i : null);
            }

            var returnType = target.ReturnType;
            if (returnDeclaringInstance)
            {
                returnType = declaringType;
            }

            generator.EmitIL(
                    condition: declaringType.IsValueType || target.IsStatic,
                    emitThis: il => il.EmitCall(OpCodes.Call, target, GetOptionalArgsOrNull(target)),
                    elseEmit: il => il.EmitCall(OpCodes.Callvirt, target, GetOptionalArgsOrNull(target)))
                .EmitIL(il => il.Emit(OpCodes.Ldloc_0), condition: returnDeclaringInstance && declaringType.IsValueType)
                .EmitIL(il => il.Emit(OpCodes.Ldarg_0), condition: returnDeclaringInstance && !declaringType.IsValueType)
                .EmitIL(il => il.Emit(OpCodes.Box, returnType), condition: returnType.IsValueType && returnType != typeof(void))
                .Emit(OpCodes.Ret);
        }

        private static void EmitLoadAndCast(this ILGenerator generator, Type targetArgType, byte argIndex, int? arrayIndex = null)
        {
            Contract.Requires(targetArgType != null);

            generator.EmitIL(
                       condition: OpcodesvLdArg.ContainsIndex(argIndex),
                       emitThis: il => il.Emit(OpcodesvLdArg[argIndex]),
                       elseEmit: il => il.Emit(OpCodes.Ldarg, argIndex))
                .EmitIL(
                    condition: arrayIndex.HasValue,
                    emitTo: il =>
                        {
                            var i = arrayIndex.Value;
                            il.EmitIL(
                                condition: OpCodesvLdcI4.ContainsIndex(i),
                                emitThis: il2 => il2.Emit(OpCodesvLdcI4[i]),
                                elseEmit: il2 => il2.Emit(OpCodes.Ldc_I4, i))
                            .Emit(OpCodes.Ldelem_Ref);
                        })
                .EmitIL(
                    condition: targetArgType.IsValueType,
                    emitThis: il => il.Emit(OpCodes.Unbox_Any, targetArgType),
                    elseEmit: il => il.Emit(OpCodes.Castclass, targetArgType));
        }

        private static Type[] GetOptionalArgsOrNull(MethodBase target)
        {
            Contract.Requires(target != null);

            if (target.CallingConvention.HasFlag(CallingConventions.VarArgs))
            {
                return target.GetParameters().Where(p => p.IsOptional).Select(p => p.ParameterType).ToArray();
            }

            return null;
        }

        private static void Validate(this ParameterInfo[] parameters)
        {
            Contract.Requires(parameters != null);

            for (int i = 0; i < parameters.GetLength(0); ++i)
            {
                if (parameters[i].IsOut)
                {
                    throw new InvalidOperationException(
                        string.Format(
                        "Out parameters are not supported. Param {0} for type {1}.",
                        parameters[i].Name,
                        parameters[i].Member.DeclaringType));
                }
            }
        }

        private static ILGenerator EmitIL(this ILGenerator generator, Action<ILGenerator> emitTo, bool condition)
        {
            Contract.Requires(!condition || emitTo != null);

            if (condition)
            {
                emitTo(generator);
            }

            return generator;
        }

        private static ILGenerator EmitIL(this ILGenerator generator, bool condition, Action<ILGenerator> emitThis, Action<ILGenerator> elseEmit = null)
        {
            Contract.Requires(!condition || emitThis != null);

            if (condition)
            {
                emitThis(generator);
            }
            else
            {
                elseEmit?.Invoke(generator);
            }

            return generator;
        }

        private static ILGenerator EmitIL(this ILGenerator generator, Action<ILGenerator> generationAction)
        {
            Contract.Requires(generationAction != null);

            generationAction(generator);
            return generator;
        }

        private static bool ContainsIndex<T>(this T[] array, int index)
        {
            Contract.Requires(array != null);

            return index >= array.GetLowerBound(0) && index <= array.GetUpperBound(0);
        }

        private static DynamicMethod BuildMethod(Type declaringType, Type[] args, Type returnType)
        {
            Contract.Requires(args != null);
            Contract.Requires(returnType != null);
            Contract.Ensures(Contract.Result<DynamicMethod>() != null);

            declaringType = declaringType?.IsAbstract == true ? typeof(object) : declaringType ?? typeof(object);
            return new DynamicMethod("_" + Guid.NewGuid().ToString("X"), returnType, args, declaringType, SkipJitVerifications);
        }
    }
}