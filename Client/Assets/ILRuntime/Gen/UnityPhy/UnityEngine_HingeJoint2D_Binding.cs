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
    unsafe class UnityEngine_HingeJoint2D_Binding
    {
        public static void Register(ILRuntime.Runtime.Enviorment.AppDomain app)
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
            MethodBase method;
            FieldInfo field;
            Type[] args;
            Type type = typeof(UnityEngine.HingeJoint2D);
            args = new Type[]{};
            method = type.GetMethod("get_limits", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_limits_0);
            args = new Type[]{typeof(UnityEngine.JointAngleLimits2D)};
            method = type.GetMethod("set_limits", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_limits_1);
            args = new Type[]{};
            method = type.GetMethod("get_motor", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, get_motor_2);
            args = new Type[]{typeof(UnityEngine.JointMotor2D)};
            method = type.GetMethod("set_motor", flag, null, args, null);
            app.RegisterCLRMethodRedirection(method, set_motor_3);


        }


        static StackObject* get_limits_0(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.HingeJoint2D instance_of_this_method;
            instance_of_this_method = (UnityEngine.HingeJoint2D)typeof(UnityEngine.HingeJoint2D).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.limits;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* set_limits_1(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.JointAngleLimits2D value = (UnityEngine.JointAngleLimits2D)typeof(UnityEngine.JointAngleLimits2D).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.HingeJoint2D instance_of_this_method;
            instance_of_this_method = (UnityEngine.HingeJoint2D)typeof(UnityEngine.HingeJoint2D).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.limits = value;

            return __ret;
        }

        static StackObject* get_motor_2(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 1);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.HingeJoint2D instance_of_this_method;
            instance_of_this_method = (UnityEngine.HingeJoint2D)typeof(UnityEngine.HingeJoint2D).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            var result_of_this_method = instance_of_this_method.motor;

            return ILIntepreter.PushObject(__ret, __mStack, result_of_this_method);
        }

        static StackObject* set_motor_3(ILIntepreter __intp, StackObject* __esp, IList<object> __mStack, CLRMethod __method, bool isNewObj)
        {
            ILRuntime.Runtime.Enviorment.AppDomain __domain = __intp.AppDomain;
            StackObject* ptr_of_this_method;
            StackObject* __ret = ILIntepreter.Minus(__esp, 2);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 1);
            UnityEngine.JointMotor2D value = (UnityEngine.JointMotor2D)typeof(UnityEngine.JointMotor2D).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);
            ptr_of_this_method = ILIntepreter.Minus(__esp, 2);
            UnityEngine.HingeJoint2D instance_of_this_method;
            instance_of_this_method = (UnityEngine.HingeJoint2D)typeof(UnityEngine.HingeJoint2D).CheckCLRTypes(StackObject.ToObject(ptr_of_this_method, __domain, __mStack));
            __intp.Free(ptr_of_this_method);

            instance_of_this_method.motor = value;

            return __ret;
        }



    }
}
