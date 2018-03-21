using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ILRuntime.Runtime.Stack;
using ILRuntime.Reflection;
using ILRuntime.CLR.Utils;

namespace ILRuntime.Runtime.Generated
{
    unsafe class PressEventTrigger_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(PressEventTrigger);

            field = type.GetField("IsOn", flag);
            app.RegisterCLRFieldGetter(field, get_IsOn_0);
            app.RegisterCLRFieldSetter(field, set_IsOn_0);


        }



        static object get_IsOn_0(ref object o)
        {
            return ((PressEventTrigger)o).IsOn;
        }
        static void set_IsOn_0(ref object o, object v)
        {
            ((PressEventTrigger)o).IsOn = (System.Boolean)v;
        }


    }
}
