using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teaq.Tests.FastReflection.SupportingTypes
{
    public class DisassembleTemplate
    {
        public int FieldValue;

        public string FieldRefValue;

        public int Value { get; set; }

        public string RefValue { get; set; }

        public static object Invoke(object target, object[] args)
        {
            return ((DisassembleTemplate)target).DoSomething((int)args[0], (string)args[1], (DBNull)args[2]);
        }

        public static object InvokeSomethingRediculous(object target, object[] args)
        {
            return ((DisassembleTemplate)target).DoSomethingRediculous((int)args[0], (string)args[1], (string)args[2], (string)args[3], (string)args[4], (string)args[5], (string)args[6], (string)args[7], (string)args[8], (string)args[9], (string)args[10]);
        }

        public static object InvokeOptionalAll(object target, object[] args)
        {
            return ((DisassembleTemplate)target).DoSomethingOptional((int)args[0], (string)args[1], (string)args[2]);
        }

        public static object InvokeOptionalSome(object target, object[] args)
        {
            return ((DisassembleTemplate)target).DoSomethingOptional((int)args[0], (string)args[1]);
        }

        public static void InvokeAction(object target, object[] args)
        {
            ((DisassembleTemplate)target).DoNothing((string)args[0]);
        }

        public static object InvokeSetter(object target, object value)
        {
            ((DisassembleTemplate)target).Value = (int)value;
            return target;
        }

        public static object InvokeGetter(object target)
        {
            return ((DisassembleTemplate)target).Value;
        }

        public static object InvokeGetterFieldOnStruct(object target)
        {
            var lv = (DisassembleStruct)target;
            return lv.FieldValue;
        }

        public static object InvokeSetterOnStruct(object target, object value)
        {
            var lv = (DisassembleStruct)target;
            lv.Value = (int)value;
            return lv;
        }

        public static object InvokeSetterOnClassValueField(object target, object value)
        {
            ((DisassembleTemplate)target).FieldValue = (int)value;
            return target;
        }

        public static void InvokeActionByRef(object target, object[] args)
        {
            var refArg = (string)args[0];
            ((DisassembleTemplate)target).DoSomethingByRef(ref refArg);
        }

        public static object BuildStruct(object[] args)
        {
            return new DisassembleStruct((int)args[0]);
        }

        public static object BuildStructWithDefaultCtor(object[] args)
        {
            return new DisassembleStruct();
        }

        public string DoSomethingRediculous(int arg0, string arg1, string arg2, string arg3, string arg4, string arg5, string arg6, string arg7, string arg8, string arg9, string arg10)
        {
            return arg0.ToString() + ":" + arg1 + ":" + arg2 + ":" + arg3 + ":" + arg4 + ":" + arg5 + ":" + arg6 + ":" + arg7 + ":" + arg8 + ":" + arg9 + ":" + arg10;
        }

        public string DoSomethingOptional(int arg0, string arg1, string arg2 = null)
        {
            return arg0.ToString() + ":" + arg1 + ":" + arg2;
        }

        public string DoSomething(int arg0, string arg1, DBNull arg2)
        {
            return arg0.ToString() + ":" + arg1 + ":" + arg2;
        }

        public void DoNothing(string arg)
        {
            arg = "Test";
        }

        public void DoSomethingByRef(ref string arg)
        {
            arg = "Test";
        }

        public void DoSomethingByOut(out string arg)
        {
            arg = "Test";
        }
    }

    public struct DisassembleStruct
    {
        public DisassembleStruct(int value) : this()
        {
            this.Value = value;
        }

        public int FieldValue;

        public string FieldString;

        public int Value { get; set; }
    }
}
