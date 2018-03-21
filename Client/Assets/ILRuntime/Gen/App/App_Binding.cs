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
    unsafe class App_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(App);
            args = new Type[] { };
            method = type.GetMethod("get_Instance", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_Instance_0);
            args = new Type[] { };
            method = type.GetMethod("getPlayer1", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, getPlayer1_1);
            args = new Type[] { };
            method = type.GetMethod("getPlayer2", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, getPlayer2_2);
            args = new Type[] { };
            method = type.GetMethod("getPlayer1Pic", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, getPlayer1Pic_3);
            args = new Type[] { };
            method = type.GetMethod("getPlayer2Pic", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, getPlayer2Pic_4);
            args = new Type[] { };
            method = type.GetMethod("getRoomId", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, getRoomId_5);
            args = new Type[] { };
            method = type.GetMethod("getToken", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, getToken_6);
            args = new Type[] { };
            method = type.GetMethod("getServer", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, getServer_7);
            args = new Type[] { typeof(System.String) };
            method = type.GetMethod("end", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, end_8);
            args = new Type[] { typeof(System.String) };
            method = type.GetMethod("commonFunction", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, commonFunction_9);
            args = new Type[] { typeof(System.Collections.IEnumerator) };
            method = type.GetMethod("DoCoroutine", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, DoCoroutine_10);
            args = new Type[] { };
            method = type.GetMethod("StopUnityCoroutines", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, StopUnityCoroutines_11);
            args = new Type[] { };
            method = type.GetMethod("Over", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, Over_12);
            args = new Type[] { typeof(System.String), typeof(System.Int32), typeof(System.Single), typeof(System.Single), typeof(NetWorkType) };
            method = type.GetMethod("InitNetWork", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, InitNetWork_13);
            args = new Type[] { };
            method = type.GetMethod("EndNetWork", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, EndNetWork_14);

            field = type.GetField("network", flag);
            app.RegisterCLRFieldGetter(field, get_network_0);
            app.RegisterCLRFieldSetter(field, set_network_0);


        }


        static StackObject* get_Instance_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 0);

            var result_of_this_method = App.Instance;

            object obj_result_of_this_method = result_of_this_method;
            if (obj_result_of_this_method is CrossBindingAdaptorType)
            {
                return ILIntepreter.PushObject(__ret, __mStack, ((CrossBindingAdaptorType)obj_result_of_this_method).ILInstance);
            }
            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* getPlayer1_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            App instance_of_this_method;
            instance_of_this_method = (App)typeof(App).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.getPlayer1();

            return __ret;
        }

        static StackObject* getPlayer2_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            App instance_of_this_method;
            instance_of_this_method = (App)typeof(App).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.getPlayer2();

            return __ret;
        }

        static StackObject* getPlayer1Pic_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            App instance_of_this_method;
            instance_of_this_method = (App)typeof(App).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.getPlayer1Pic();

            return __ret;
        }

        static StackObject* getPlayer2Pic_4(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            App instance_of_this_method;
            instance_of_this_method = (App)typeof(App).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.getPlayer2Pic();

            return __ret;
        }

        static StackObject* getRoomId_5(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            App instance_of_this_method;
            instance_of_this_method = (App)typeof(App).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.getRoomId();

            return __ret;
        }

        static StackObject* getToken_6(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            App instance_of_this_method;
            instance_of_this_method = (App)typeof(App).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.getToken();

            return __ret;
        }

        static StackObject* getServer_7(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            App instance_of_this_method;
            instance_of_this_method = (App)typeof(App).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.getServer();

            return __ret;
        }

        static StackObject* end_8(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String result = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            App instance_of_this_method;
            instance_of_this_method = (App)typeof(App).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.end(result);

            return __ret;
        }

        static StackObject* commonFunction_9(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.String result = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            App instance_of_this_method;
            instance_of_this_method = (App)typeof(App).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.commonFunction(result);

            return __ret;
        }

        static StackObject* DoCoroutine_10(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            System.Collections.IEnumerator coroutine = (System.Collections.IEnumerator)typeof(System.Collections.IEnumerator).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            App instance_of_this_method;
            instance_of_this_method = (App)typeof(App).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.DoCoroutine(coroutine);

            return __ret;
        }

        static StackObject* StopUnityCoroutines_11(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            App instance_of_this_method;
            instance_of_this_method = (App)typeof(App).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.StopUnityCoroutines();

            return __ret;
        }

        static StackObject* Over_12(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            App instance_of_this_method;
            instance_of_this_method = (App)typeof(App).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.Over();

            return __ret;
        }

        static StackObject* InitNetWork_13(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 6);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            NetWorkType type = (NetWorkType)typeof(NetWorkType).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            System.Single _DisConnTime = *(float*)&ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 3);
            System.Single _HeartTime = *(float*)&ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 4);
            System.Int32 port = ptr_of_this_method->Value;
            ptr_of_this_method = ILIntepreter.Minus(__esp, 5);
            System.String ip = (System.String)typeof(System.String).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 6);
            App instance_of_this_method;
            instance_of_this_method = (App)typeof(App).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.InitNetWork(ip, port, _HeartTime, _DisConnTime, type);

            return __ret;
        }

        static StackObject* EndNetWork_14(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            App instance_of_this_method;
            instance_of_this_method = (App)typeof(App).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.EndNetWork();

            return __ret;
        }


        static object get_network_0(ref object o)
        {
            return ((App)o).network;
        }
        static void set_network_0(ref object o, object v)
        {
            ((App)o).network = (NetBase)v;
        }


    }
}
