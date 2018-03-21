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
    unsafe class NetBase_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(NetBase);
            args = new Type[]{typeof(System.UInt32), typeof(System.UInt32), typeof(System.Int32), typeof(System.Byte[])};
            method = type.GetMethod("Send", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Send_0);
            args = new Type[]{typeof(System.UInt32), typeof(System.UInt32), typeof(System.Int32), typeof(System.Byte[])};
            method = type.GetMethod("GetData", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, GetData_1);
            args = new Type[]{typeof(System.Int32), typeof(System.Action<NetBase, System.Byte[], System.Int32>)};
            method = type.GetMethod("AddHandle", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, AddHandle_2);
            args = new Type[]{};
            method = type.GetMethod("ClearConnEvent", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, ClearConnEvent_3);
            args = new Type[]{typeof(System.UInt32)};
            method = type.GetMethod("Conn", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Conn_4);

            field = type.GetField("ConnectResultEvent", flag);
            app.RegisterCLRFieldGetter(field, get_ConnectResultEvent_0);
            app.RegisterCLRFieldSetter(field, set_ConnectResultEvent_0);
            field = type.GetField("DisConnectEvent", flag);
            app.RegisterCLRFieldGetter(field, get_DisConnectEvent_1);
            app.RegisterCLRFieldSetter(field, set_DisConnectEvent_1);
            field = type.GetField("HeartEvent", flag);
            app.RegisterCLRFieldGetter(field, get_HeartEvent_2);
            app.RegisterCLRFieldSetter(field, set_HeartEvent_2);


        }


        static StackObject* Send_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 5);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Byte[] datas = (System.Byte[])typeof(System.Byte[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 cmd = ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.UInt32 protocol = (uint)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.UInt32 id = (uint)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            NetBase instance_of_this_method;
            instance_of_this_method = (NetBase)typeof(NetBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Send(id, protocol, cmd, datas);

            return __ret;
        }

        static StackObject* GetData_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 5);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Byte[] msg = (System.Byte[])typeof(System.Byte[]).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 cmd = ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.UInt32 protocol = (uint)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.UInt32 id = (uint)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            NetBase instance_of_this_method;
            instance_of_this_method = (NetBase)typeof(NetBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.GetData(id, protocol, cmd, msg);

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* AddHandle_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 3);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Action<NetBase, System.Byte[], System.Int32> func = (System.Action<NetBase, System.Byte[], System.Int32>)typeof(System.Action<NetBase, System.Byte[], System.Int32>).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Int32 cmd = ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            NetBase instance_of_this_method;
            instance_of_this_method = (NetBase)typeof(NetBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.AddHandle(cmd, func);

            return __ret;
        }

        static StackObject* ClearConnEvent_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            NetBase instance_of_this_method;
            instance_of_this_method = (NetBase)typeof(NetBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.ClearConnEvent();

            return __ret;
        }

        static StackObject* Conn_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.UInt32 conv = (uint)ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            NetBase instance_of_this_method;
            instance_of_this_method = (NetBase)typeof(NetBase).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Conn(conv);

            return __ret;
        }


        static object get_ConnectResultEvent_0(ref object o)
        {
            return ((NetBase)o).ConnectResultEvent;
        }
        static void set_ConnectResultEvent_0(ref object o, object v)
        {
            ((NetBase)o).ConnectResultEvent = (System.Action<System.Boolean>)v;
        }
        static object get_DisConnectEvent_1(ref object o)
        {
            return ((NetBase)o).DisConnectEvent;
        }
        static void set_DisConnectEvent_1(ref object o, object v)
        {
            ((NetBase)o).DisConnectEvent = (System.Action)v;
        }
        static object get_HeartEvent_2(ref object o)
        {
            return ((NetBase)o).HeartEvent;
        }
        static void set_HeartEvent_2(ref object o, object v)
        {
            ((NetBase)o).HeartEvent = (System.Action)v;
        }


    }
}
