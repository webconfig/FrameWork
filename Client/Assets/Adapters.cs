using ILRuntime.Runtime.Enviorment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILRuntime.Runtime.Intepreter;
using System.Collections;

namespace Assets
{
	class IEnumerableAdapter : CrossBindingAdaptor
	{
		public override Type BaseCLRType
		{
			get
			{
				return typeof(System.Collections.IEnumerable);
			}
		}

		public override Type AdaptorType
		{
			get { return typeof(Adaptor); }
		}

		public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
		{
			return new Adaptor(appdomain, instance);
		}

		internal class Adaptor : System.Collections.IEnumerable, CrossBindingAdaptorType
		{
			ILTypeInstance instance;
			ILRuntime.Runtime.Enviorment.AppDomain appdomain;

			public Adaptor() : base()
			{

			}

			public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
			{
				this.appdomain = appdomain;
				this.instance = instance;
				Init();
			}

			public ILTypeInstance ILInstance { get { return instance; } }

			ILRuntime.CLR.Method.IMethod mMethoGetExObject;
			void Init()
			{
				mMethoGetExObject = instance.Type.GetMethod("GetEnumerator", 0);
			}

			public IEnumerator GetEnumerator()
			{
				return appdomain.Invoke(mMethoGetExObject, instance) as IEnumerator;
			}
		}
	}

	class IDisposableAdapter : CrossBindingAdaptor
	{
		public override Type BaseCLRType
		{
			get
			{
				return typeof(IDisposable);
			}
		}

		public override Type AdaptorType { get { return typeof(Adaptor); } }

		public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
		{
			return new Adaptor(appdomain, instance);
		}

		internal class Adaptor : IDisposable, CrossBindingAdaptorType
		{
			ILTypeInstance instance;
			ILRuntime.Runtime.Enviorment.AppDomain appdomain;

			public Adaptor() : base()
			{

			}

			public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
			{
				this.appdomain = appdomain;
				this.instance = instance;
				Init();
			}

			public ILTypeInstance ILInstance { get { return instance; } }

			ILRuntime.CLR.Method.IMethod mMethoGetExObject;
			void Init()
			{
				mMethoGetExObject = instance.Type.GetMethod("Dispose", 0);
			}

			

			public void Dispose()
			{
				appdomain.Invoke(mMethoGetExObject, instance);
			}
		}
	}

	class ExceptionAdapter : CrossBindingAdaptor
	{
		public override Type BaseCLRType
		{
			get
			{
				return typeof(Exception);
			}
		}

		public override Type AdaptorType { get { return typeof(Adaptor); } }

		public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
		{
			return new Adaptor(appdomain, instance);
		}

		internal class Adaptor : Exception, CrossBindingAdaptorType
		{
			ILTypeInstance instance;
			ILRuntime.Runtime.Enviorment.AppDomain appdomain;

			public Adaptor() : base()
			{

			}

			public Adaptor(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
			{
				this.appdomain = appdomain;
				this.instance = instance;
				Init();
			}

			public ILTypeInstance ILInstance { get { return instance; } }

			
			void Init()
			{
				
			}

		}
	}
}
