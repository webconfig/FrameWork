#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

[System.Reflection.Obfuscation(Exclude = true)]
public class ILRuntimeCLRBinding
{
    [MenuItem("ILRuntime/Generate CLR Binding Code")]
    static void GenerateCLRBinding()
    {
        List<Type> types = new List<Type>();
        //=====数据=====
        //types.Add(typeof(int));
        //types.Add(typeof(Int32));
        //types.Add(typeof(Int64));
        //types.Add(typeof(float));
        //types.Add(typeof(long));
        //types.Add(typeof(object));
        //types.Add(typeof(String));
        //types.Add(typeof(Char));
        //types.Add(typeof(Boolean));
        //types.Add(typeof(Byte));
        //types.Add(typeof(Array));
        //types.Add(typeof(Buffer));
        //types.Add(typeof(DateTime));
        //===2
        //types.Add(typeof(UInt32));


        //types.Add(typeof(List<int>));
        //types.Add(typeof(List<float>));
        //types.Add(typeof(List<long>));
        //types.Add(typeof(List<string>));
        //types.Add(typeof(List<Vector2>));
        //types.Add(typeof(List<Vector3>));
        //types.Add(typeof(List<GameObject>));


        //===2


        //types.Add(typeof(Dictionary<int, GameObject>));
        //types.Add(typeof(Dictionary<int, App>));
        //types.Add(typeof(Dictionary<string, GameObject>));
        //types.Add(typeof(Dictionary<string, App>));
        //////====system===
        //types.Add(typeof(BitConverter));
        //types.Add(typeof(MemoryStream));
        //types.Add(typeof(Stream));
        //types.Add(typeof(System.Math));
        //types.Add(typeof(Convert));

        //////====unity=====
        //types.Add(typeof(Vector2));
        //types.Add(typeof(Vector3));
        //types.Add(typeof(Quaternion));
        //types.Add(typeof(Mathf));
        //types.Add(typeof(UnityEngine.Object));
        //types.Add(typeof(Time));
        //types.Add(typeof(Input));
        //types.Add(typeof(Quaternion));
        //types.Add(typeof(Shader));
        //types.Add(typeof(RectTransform));
        //===2
        //types.Add(typeof(UnityEngine.Random));
        //types.Add(typeof(UnityEngine.Rect));
        //==3
        //types.Add(typeof(UnityEngine.ParticleSystem));
        //==4
        //types.Add(typeof(UnityEngine.Material));
        //types.Add(typeof(UnityEngine.Color));
        //types.Add(typeof(UnityEngine.Microphone));
        //types.Add(typeof(UnityEngine.WWW));
        types.Add(typeof(UnityEngine.LineRenderer));

        ////==
        //types.Add(typeof(GameObject));
        //types.Add(typeof(Transform));
        //types.Add(typeof(RectTransform));
        //types.Add(typeof(Animation));
        //types.Add(typeof(AudioClip));
        //types.Add(typeof(AudioSource));
        //types.Add(typeof(Camera));
        //types.Add(typeof(Component));
        //types.Add(typeof(Debug));
        //==2
        //types.Add(typeof(Animator));

        ////== unity ui
        //types.Add(typeof(UnityEngine.UI.Button));
        //types.Add(typeof(UnityEngine.UI.Image));
        //types.Add(typeof(UnityEngine.UI.Text));
        //===2
        //types.Add(typeof(UnityEngine.Canvas));

        ////===物理
        //types.Add(typeof(Rigidbody2D));
        //types.Add(typeof(BoxCollider2D));
        //types.Add(typeof(CircleCollider2D));

        //所有DLL内的类型的真实C#类型都是ILTypeInstance
        types.Add(typeof(List<ILRuntime.Runtime.Intepreter.ILTypeInstance>));

        ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(types, "Assets/ILRuntime/Generated");

    }

    [MenuItem("ILRuntime/Generate CLR Binding Code by Analysis")]
    static void GenerateCLRBindingByAnalysis()
    {
        //用新的分析热更dll调用引用来生成绑定代码
        ILRuntime.Runtime.Enviorment.AppDomain domain = new ILRuntime.Runtime.Enviorment.AppDomain();
        using (System.IO.FileStream fs = new System.IO.FileStream("Assets/StreamingAssets/HotFix_Project.dll", System.IO.FileMode.Open, System.IO.FileAccess.Read))
        {
            domain.LoadAssembly(fs);
        }
        //Crossbind Adapter is needed to generate the correct binding code
        InitILRuntime(domain);
        ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, "Assets/ILRuntime/Generated");
    }

    [MenuItem("ILRuntime/Get CLR")]
    static void GetCLR()
    {
        string str = "";
        str += GetStr("Assets/ILRuntime/Gen/Data");
        str += GetStr("Assets/ILRuntime/Gen/Sys");
        str += GetStr("Assets/ILRuntime/Gen/List");
        str += GetStr("Assets/ILRuntime/Gen/Dictionary");
        str += GetStr("Assets/ILRuntime/Gen/Thread");
        str += GetStr("Assets/ILRuntime/Gen/Protobuf");
        str += GetStr("Assets/ILRuntime/Gen/Net");

        str += GetStr("Assets/ILRuntime/Gen/Unity");
        str += GetStr("Assets/ILRuntime/Gen/UnityComp");
        str += GetStr("Assets/ILRuntime/Gen/UnityEvent");
        str += GetStr("Assets/ILRuntime/Gen/UnityUI");
        str += GetStr("Assets/ILRuntime/Gen/UnityPhy");
        str += GetStr("Assets/ILRuntime/Gen/UnityRender");
        str += GetStr("Assets/ILRuntime/Gen/App");
        Debug.Log(str);
    }
    private static string GetStr(string path)
    {
        DirectoryInfo TheFolder = new DirectoryInfo(path);
        string str = "";
        //遍历文件
        foreach (FileInfo NextFile in TheFolder.GetFiles())
        {
            if (NextFile.Name.IndexOf(".meta") < 0)
            {
                if (NextFile.Name.IndexOf("Android") >= 0)
                {
                    str += "#if UNITY_ANDROID\r\n";
                    str += string.Format("{0}.Register(app);\r\n", NextFile.Name.Replace(".cs", ""));
                    str += "#endif\r\n";
                }
                else
                {
                    str += string.Format("{0}.Register(app);\r\n", NextFile.Name.Replace(".cs", ""));
                }
            }
        }
        return str;
    }

    static void InitILRuntime(ILRuntime.Runtime.Enviorment.AppDomain domain)
    {
        //这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用
        domain.RegisterCrossBindingAdaptor(new Adapt_IMessage());
        domain.DelegateManager.RegisterFunctionDelegate<Adapt_IMessage.Adaptor>();

        domain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
        //domain.RegisterCrossBindingAdaptor(new InheritanceAdapter());

    }
}
#endif
