using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Ionic.Zip;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.CLR.Method;
using System.Collections;

public class App : MonoBehaviour
{
    private static App _instance;
    public static App Instance
    {
        get
        {
            return _instance;
        }
    }

    public GameObject Loading;
    public NetBase network;
    public Text text;
    public ILRuntime.Runtime.Enviorment.AppDomain appdomain;
    [System.NonSerialized]
    public int _run = 0;
    public RunTyp type;
    private string data_path;

    public Game game;

    private void Awake()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        _instance = this;
        if (!Loading.activeInHierarchy)
        {
            Loading.SetActive(true);
        }
        text.text = "Loading...";
    }

    void Start()
    {
        load_game();
        game = new Game();
        game.Init();
        game.world.InitDebug(mat_green, mat_red);
    }

    private void Update()
    {
        game.world.OnGUI();
        if (_run == -1)
        {//游戏结束
            Debug.LogWarning("==========游戏结束清理资源开始=============");
            _run = 0;
            StopAllCoroutines();//结束所有携程
            method = null;
            method_update = null;
            method_out = null;
            appdomain = null;
            //AssetbundleLoader.Clear();//释放资源
            EndNetWork();
            Debug.LogWarning("==========游戏结束清理资源结束=============");
        }
        else if (_run > 0)
        {
            if (network != null)
            {
                network.Update();
            }
            appdomain.Invoke(method_update, null, null);
        }
    }

    #region 网络
    public void InitNetWork(string ip, int port, float _HeartTime, float _DisConnTime, NetWorkType type)
    {
        EndNetWork();
        network = new NetBase(ip, port, _HeartTime, _DisConnTime, type);
    }
    public void EndNetWork()
    {
        if (network != null)
        {
            network.End();
            network = null;
        }
    }
    #endregion

    private void load_game()
    {
#if UNITY_EDITOR
        data_path = Application.streamingAssetsPath;
#elif UNITY_ANDROID
        data_path = Application.temporaryCachePath;
#elif UNITY_IOS
        data_path = Application.temporaryCachePath;
#endif
        Debug.Log("开始加载游戏：" + data_path);
        //===加载热更新代码===
        appdomain = new ILRuntime.Runtime.Enviorment.AppDomain();
        //appdomain.UnityMainThreadID = Thread.CurrentThread.ManagedThreadId;//调试
        string dll_path = string.Format("{0}/{1}", data_path, "HotFix_Project.dll");
        byte[] dll = File.ReadAllBytes(dll_path);
        string dll_pdb_path = string.Format("{0}/{1}", data_path, "HotFix_Project.pdb");
        byte[] pdb = File.ReadAllBytes(dll_pdb_path);
        using (System.IO.MemoryStream fs = new MemoryStream(dll))
        {
            using (System.IO.MemoryStream p = new MemoryStream(pdb))
            {
                appdomain.LoadAssembly(fs, p, new Mono.Cecil.Pdb.PdbReaderProvider());
            }
        }
        //此处调用初始化
        InitializeILRuntime();
        //ILRuntime.Runtime.Generated.CLRBindings.Initialize(appdomain);
        Assets.CommGen.Initialize(appdomain);
        OnHotFixLoaded();
        Debug.Log("启动完成");
    }
    #region 加载代码

    void InitializeILRuntime()
    {
        //这里做一些ILRuntime的注册，HelloWorld示例暂时没有需要注册的
        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
        {
            return new UnityEngine.Events.UnityAction(() =>
            {
                ((Action)act)();
            });
        });
        appdomain.DelegateManager.RegisterMethodDelegate<NetBase, System.Byte[], System.Int32>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean>();

        appdomain.DelegateManager.RegisterMethodDelegate<System.IAsyncResult>();
        appdomain.DelegateManager.RegisterDelegateConvertor<System.AsyncCallback>((act) =>
        {
            return new System.AsyncCallback((ar) =>
            {
                ((Action<System.IAsyncResult>)act)(ar);
            });
        });
        appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, System.Byte[]>();
        appdomain.DelegateManager.RegisterDelegateConvertor<System.Threading.ThreadStart>((act) =>
        {
            return new System.Threading.ThreadStart(() =>
            {
                ((Action)act)();
            });
        });
        //====proto=======
        appdomain.RegisterCrossBindingAdaptor(new Adapt_IMessage());
        appdomain.DelegateManager.RegisterFunctionDelegate<Adapt_IMessage.Adaptor>();
        appdomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
        LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(appdomain);

        //预先获得IMethod，可以减低每次调用查找方法耗用的时间
        IType type = appdomain.LoadedTypes["HotFix_Project.Main"];
        //根据方法名称和参数个数获取方法
        method = type.GetMethod("Run", 0);
        method_update = type.GetMethod("Update", 0);
        method_out = type.GetMethod("Out", 0);
    }
    private IMethod method, method_update, method_out;
    /// <summary>
    /// 启动代码
    /// </summary>
    void OnHotFixLoaded()
    {
        appdomain.Invoke(method, null, null);
        _run = 1;
        Loading.gameObject.SetActive(false);
    }
    #endregion

    #region 热更新工程接口
    public void Over()
    {
        _run = -1;
        Loading.gameObject.SetActive(true);
        text.text = "";
    }
    public void DoCoroutine(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
    public void StopUnityCoroutines()
    {
        StopAllCoroutines();//结束所有携程
    }
    #endregion

    //==========
    public Material mat_green, mat_red;
    private void OnGUI()
    {
        if(GUI.Button(new Rect(100, 200, 100, 100), "地图生成"))
        {
            game.world.RayCast();
        }
    }


}
public enum RunTyp
{
    发布正式= 0,
    发布覆盖测试 = 1,
    本地覆盖测试 = 2,
    不覆盖运行=3
}

