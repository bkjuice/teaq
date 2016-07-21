//#define EMIT_IL_ASSEMBLY

using System;
#if EMIT_IL_ASSEMBLY
using System.Reflection;
using System.Reflection.Emit;
#endif
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Teaq.Tests.FastReflection.SupportingTypes;
using Teaq.FastReflection;

namespace Teaq.Tests
{
    [TestClass]
    public class ILGeneratorTests
    {

        [TestMethod]
        public void Create_struct_instance_with_parameterized_ctor()
        {
            var target = typeof(DisassembleStruct).GetConstructor(new[] { typeof(int) });

#if EMIT_IL_ASSEMBLY
            Action<ILGenerator> writeILAction = il =>
            {
                il.ReflectEmitConstructor(target);
            };

            WriteMethodILToAssembly(writeILAction, "Create_struct_instance_with_parameterized_ctor", "TestClass", target);
#endif

            var instance = target.ReflectConstructor()(new object[] { 50 });
            Assert.AreEqual(50, ((DisassembleStruct)instance).Value);
        }

        [TestMethod]
        public void Create_instance_with_default_ctor()
        {
            var target = typeof(DisassembleTemplate).GetConstructor(Type.EmptyTypes);

#if EMIT_IL_ASSEMBLY
            Action<ILGenerator> writeILAction = il =>
            {
                il.ReflectEmitConstructor(target);
            };
            
            WriteMethodILToAssembly(writeILAction, "Create_instance_with_default_ctor", "TestClass", target);
#endif

            var instance = target.ReflectConstructor()(null);
        }

        [TestMethod]
        public void Initialize_value_type()
        {
            var instance = typeof(DisassembleStruct).ReflectValueInitializer()();
            Assert.AreEqual(0, ((DisassembleStruct)instance).Value);
        }

        [TestMethod]
        public void ReflectMethod_invokes_expected_method_and_returns_expected_value()
        {
            // Establish expected results:
            var testArgs = new object[] { 1, "Test", DBNull.Value };
            var explicitTarget = new DisassembleTemplate();
            var expected = DisassembleTemplate.Invoke(explicitTarget, testArgs);

            var targetType = typeof(DisassembleTemplate);
            var targetMethod = targetType.GetMethod("DoSomething");
            
#if EMIT_IL_ASSEMBLY
            // Written as a delegate to allow for reuse when writing IL to file:
            Action<ILGenerator> writeILAction = il =>
            {
                il.ReflectEmitMethodProxy(targetMethod);
            };

            var methodTemplate = targetType.GetMethod("Invoke");
            WriteMethodILToAssembly(writeILAction, "ReflectMethod_invokes_expected_method_and_returns_expected_value", "TestClass", targetMethod);
#endif

            var result = targetMethod.ReflectFunc()(explicitTarget, testArgs).ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ReflectMethod_with_11_params_invokes_expected_method_and_returns_expected_value()
        {
            // Establish expected results:
            var testArgs = new object[] { 1, "2", "3", "4", "5", "6", "7", "8", "9", "10", "11"};
            var explicitTarget = new DisassembleTemplate();
            var expected = DisassembleTemplate.InvokeSomethingRediculous(explicitTarget, testArgs);

            var targetType = typeof(DisassembleTemplate);
            var targetMethod = targetType.GetMethod("DoSomethingRediculous");

#if EMIT_IL_ASSEMBLY
            // Written as a delegate to allow for reuse when writing IL to file:
            Action<ILGenerator> writeILAction = il =>
            {
                il.ReflectEmitMethodProxy(targetMethod);
            };

            var methodTemplate = targetType.GetMethod("Invoke");
            WriteMethodILToAssembly(writeILAction, "ReflectMethod_with_11_params_invokes_expected_method_and_returns_expected_value", "TestClass", targetMethod);
#endif

            var result = targetMethod.ReflectFunc()(explicitTarget, testArgs).ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ReflectMethod_invokes_expected_method_with_all_optional_params_and_returns_expected_value()
        {
            // Establish expected results:
            var testArgs = new object[] { 1, "Test", "_andDone" };
            var explicitTarget = new DisassembleTemplate();
            var expected = DisassembleTemplate.InvokeOptionalAll(explicitTarget, testArgs);

            var targetType = typeof(DisassembleTemplate);
            var targetMethod = targetType.GetMethod("DoSomethingOptional");

           

#if EMIT_IL_ASSEMBLY
            Action<ILGenerator> writeILAction = il =>
            {
                il.ReflectEmitMethodProxy(targetMethod);
            };

            var methodTemplate = targetType.GetMethod("Invoke");
            WriteMethodILToAssembly(writeILAction, "ReflectMethod_invokes_expected_method_with_all_optional_params_and_returns_expected_value", "TestClass", targetMethod);
#endif

            var result = targetMethod.ReflectFunc()(explicitTarget, testArgs).ToString();
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void ReflectMethod_invokes_method_with_some_optional_params_throws_IndexOutOfRangeException()
        {
            // Establish expected results:
            var testArgs = new object[] { 1, "Test"};
            var explicitTarget = new DisassembleTemplate();
            var targetType = typeof(DisassembleTemplate);
            var targetMethod = targetType.GetMethod("DoSomethingOptional");

            try
            {
                var result = targetMethod.ReflectFunc()(explicitTarget, testArgs).ToString();
                Assert.Fail("Expected IndexOutOfRangeException.");
            }
            catch (IndexOutOfRangeException)
            {
            }
        }

        [TestMethod]
        public void ReflectMethod_invokes_method_with_out_param_throws_InvalidOperationException()
        {
            // Establish expected results:
            var testArgs = new object[] { "Test" };
            var explicitTarget = new DisassembleTemplate();
            var targetType = typeof(DisassembleTemplate);
            var targetMethod = targetType.GetMethod("DoSomethingByOut");

            try
            {
                var result = targetMethod.ReflectFunc()(explicitTarget, testArgs).ToString();
                Assert.Fail("Expected InvalidOperationException.");
            }
            catch (InvalidOperationException)
            {
            }
        }

        [TestMethod]
        public void ReflectAction_invokes_expected_method()
        {
            // Establish expected results:
            var testArgs = new object[] { "Test"};
            var explicitTarget = new DisassembleTemplate();

            var targetType = typeof(DisassembleTemplate);
            var targetMethod = targetType.GetMethod("DoNothing");

            targetMethod.ReflectAction()(explicitTarget, testArgs);
        }

        [TestMethod]
        public void ReflectGetter_returns_expected_value_from_class()
        {
            // Establish expected results:
            var explicitTarget = new DisassembleTemplate();
            explicitTarget.Value = 100;

            var targetType = typeof(DisassembleTemplate);
            var target = targetType.GetProperty("Value");

#if EMIT_IL_ASSEMBLY
            Action<ILGenerator> writeILAction = il =>
            {
                il.ReflectEmitMethodProxy(target.GetGetMethod(), false);
            };

            WriteMethodILToAssembly(writeILAction, "ReflectGetter_returns_expected_value_from_class", "TestClass", target.GetGetMethod());
#endif

            var value = target.GetGetMethod().ReflectGetter()(explicitTarget);
            Assert.AreEqual(100, (int)value);
        }

        [TestMethod]
        public void ReflectGetter_returns_expected_value_from_struct()
        {
            // Establish expected results:
            var explicitTarget = new DisassembleStruct();
            explicitTarget.Value = 100;

            var targetType = typeof(DisassembleStruct);
            var target = targetType.GetProperty("Value");

#if EMIT_IL_ASSEMBLY
            Action<ILGenerator> writeILAction = il =>
            {
                il.ReflectEmitMethodProxy(target.GetGetMethod(), false);
            };

            WriteMethodILToAssembly(writeILAction, "ReflectGetter_returns_expected_value_from_struct", "TestClass", target.GetGetMethod());
#endif

            var value = target.GetGetMethod().ReflectGetter()(explicitTarget);
            Assert.AreEqual(100, (int)value);
        }

        [TestMethod]
        public void ReflectAction_invokes_setter_on_class()
        {
            // Establish expected results:
            var testArg =  1;
            var explicitTarget = new DisassembleTemplate();

            var targetType = typeof(DisassembleTemplate);
            var target = targetType.GetProperty("Value");


#if EMIT_IL_ASSEMBLY
            Action<ILGenerator> writeILAction = il =>
            {
                il.ReflectEmitMethodProxy(target.GetSetMethod(), false, true);
            };

            WriteMethodILToAssembly(writeILAction, "ReflectAction_invokes_setter_on_class", "TestClass", target.GetSetMethod());
#endif
            target.GetSetMethod().ReflectSetter(target.PropertyType)(explicitTarget, testArg);
            Assert.AreEqual(explicitTarget.Value, 1);
        }

        [TestMethod]
        public void ReflectAction_invokes_setter_on_class_field_value_type()
        {
            // Establish expected results:
            var testArg = 1;
            var explicitTarget = new DisassembleTemplate();

            var targetType = typeof(DisassembleTemplate);
            var target = targetType.GetField("FieldValue");


#if EMIT_IL_ASSEMBLY
            Action<ILGenerator> writeILAction = il =>
            {
                il.ReflectEmitFieldSetter(target);
            };

            WriteMethodILToAssembly(writeILAction, "ReflectAction_invokes_setter_on_class_field_value_type", "TestClass", target);
#endif
            target.ReflectSetter()(explicitTarget, testArg);
            Assert.AreEqual(explicitTarget.FieldValue, 1);
        }

        [TestMethod]
        public void ReflectAction_invokes_setter_on_class_field_ref_type()
        {
            // Establish expected results:
            var testArg = "TEST";
            var explicitTarget = new DisassembleTemplate();

            var targetType = typeof(DisassembleTemplate);
            var target = targetType.GetField("FieldRefValue");


#if EMIT_IL_ASSEMBLY
            Action<ILGenerator> writeILAction = il =>
            {
                il.ReflectEmitFieldSetter(target);
            };

            WriteMethodILToAssembly(writeILAction, "ReflectAction_invokes_setter_on_class_field_ref_type", "TestClass", target);
#endif
            target.ReflectSetter()(explicitTarget, testArg);
            Assert.AreEqual(explicitTarget.FieldRefValue, "TEST");
        }

        [TestMethod]
        public void ReflectAction_invokes_setter_on_struct()
        {
            // Establish expected results:
            var testArg = 55;
            object explicitTarget = new DisassembleStruct();

            var targetType = typeof(DisassembleStruct);
            var target = targetType.GetProperty("Value");

#if EMIT_IL_ASSEMBLY
            Action<ILGenerator> writeILAction = il =>
            {
                il.ReflectEmitMethodProxy(target.GetSetMethod(), false, true);
            };

            WriteMethodILToAssembly(writeILAction, "ReflectAction_invokes_setter_on_struct", "TestClass", target.GetSetMethod());
#endif

            target.GetSetMethod().ReflectSetter(target.PropertyType)(explicitTarget, testArg);
            Assert.AreEqual(((DisassembleStruct)explicitTarget).Value, 55);
        }

        [TestMethod]
        public void ReflectAction_invokes_setter_on_struct_field_value_type()
        {
            // Establish expected results:
            var testArg = 55;
            object explicitTarget = new DisassembleStruct();

            var targetType = typeof(DisassembleStruct);
            var target = targetType.GetField("FieldValue");

#if EMIT_IL_ASSEMBLY
            Action<ILGenerator> writeILAction = il =>
            {
                il.ReflectEmitFieldSetter(target);
            };

            WriteMethodILToAssembly(writeILAction, "ReflectAction_invokes_setter_on_struct_field_value_type", "TestClass", target);
#endif

            target.ReflectSetter()(explicitTarget, testArg);
            ((DisassembleStruct)explicitTarget).FieldValue.Should().Be(55);
        }

        [TestMethod]
        public void ReflectAction_invokes_setter_on_struct_field_ref_type()
        {
            // Establish expected results:
            var testArg = "TEST";

            // .NET reflection API's require this to be a boxed value:
            object explicitTarget = new DisassembleStruct();

            var targetType = typeof(DisassembleStruct);
            var target = targetType.GetField("FieldString");

#if EMIT_IL_ASSEMBLY
            Action<ILGenerator> writeILAction = il =>
            {
                il.ReflectEmitFieldSetter(target);
            };

            WriteMethodILToAssembly(writeILAction, "ReflectAction_invokes_setter_on_struct_field_ref_type", "TestClass", target);
#endif

            target.ReflectSetter()(explicitTarget, testArg);
            ((DisassembleStruct)explicitTarget).FieldString.Should().Be("TEST");
        }

        [TestMethod]
        public void ReflectGetter_returns_expected_value_from_struct_field()
        {
            // Establish expected results:
            var explicitTarget = new DisassembleStruct();
            explicitTarget.FieldValue = 100;

            var targetType = typeof(DisassembleStruct);
            var target = targetType.GetField("FieldValue");

#if EMIT_IL_ASSEMBLY
            Action<ILGenerator> writeILAction = il =>
            {
                il.ReflectEmitFieldGetter(target);
            };

            WriteMethodILToAssembly(writeILAction, "ReflectGetter_returns_expected_value_from_struct_field", "TestClass", target);
#endif

            var value = target.ReflectGetter()(explicitTarget);
            Assert.AreEqual(100, (int)value);
        }

#if EMIT_IL_ASSEMBLY
        private static void WriteMethodILToAssembly(Action<ILGenerator> writeILAction, string assemblyName, string typeName, MethodInfo info)
        {
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName, assemblyName + ".dll");

            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName);

            // Create a vararg method with no return value and one  
            // string argument. (The string argument type is the only 
            // element of an array of Type objects.) 
            //
            var attributes = MethodAttributes.Public;
            if (info.IsStatic)
            {
                attributes = attributes | MethodAttributes.Static;
            }

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                info.Name,
                attributes,
                info.CallingConvention,
                typeof(object),
                new[] { typeof(object), typeof(object[]) });

            ILGenerator il = methodBuilder.GetILGenerator();
            writeILAction(il);
            typeBuilder.CreateType();
            assemblyBuilder.Save(assemblyName + ".dll");
        }

        private static void WriteMethodILToAssembly(Action<ILGenerator> writeILAction, string assemblyName, string typeName, ConstructorInfo info)
        {
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName, assemblyName + ".dll");

            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName);

            // Create a vararg method with no return value and one  
            // string argument. (The string argument type is the only 
            // element of an array of Type objects.) 
            //
            var attributes = MethodAttributes.Public | MethodAttributes.Static;
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                "Build" + info.Name,
                attributes,
                CallingConventions.Standard,
                typeof(object),
                new[] { typeof(object[]) });

            ILGenerator il = methodBuilder.GetILGenerator();
            writeILAction(il);
            typeBuilder.CreateType();
            assemblyBuilder.Save(assemblyName + ".dll");
        }

        private static void WriteMethodILToAssembly(Action<ILGenerator> writeILAction, string assemblyName, string typeName, FieldInfo info)
        {
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName(assemblyName), AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName, assemblyName + ".dll");

            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeName);

            // Create a vararg method with no return value and one  
            // string argument. (The string argument type is the only 
            // element of an array of Type objects.) 
            //
            var attributes = MethodAttributes.Public;
            if (info.IsStatic)
            {
                attributes = attributes | MethodAttributes.Static;
            }

            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                info.Name,
                attributes,
                typeof(object),
                new[] { typeof(object), typeof(object[]) });

            ILGenerator il = methodBuilder.GetILGenerator();
            writeILAction(il);
            typeBuilder.CreateType();
            assemblyBuilder.Save(assemblyName + ".dll");
        }
#endif
    }
}
