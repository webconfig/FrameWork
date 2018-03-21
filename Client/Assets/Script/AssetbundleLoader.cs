using UnityEngine;
using System.Collections.Generic;


public class AssetbundleLoader
{
    public static string ROOT_PATH = "";
    public static string game;

    public static void Init(string data_path,string _game)
    {
        game = _game;
        ROOT_PATH = string.Format("{0}/game/{1}", data_path, game);
    }

    private static Dictionary<string, AssetBundle> _assetbundleDic = new Dictionary<string, AssetBundle>();

    /// <summary>
    /// 释放ab包资源
    /// </summary>
    public static void Clear()
    {
        var enumerator = _assetbundleDic.GetEnumerator();
        while (enumerator.MoveNext())
        {
            enumerator.Current.Value.Unload(true);
        }
        _assetbundleDic.Clear();
    }

    private static T LoadRes<T>(string bundle_name, string path) where T : Object
    {
#region 获取ab包
        AssetBundle bundle = null;
        if (!_assetbundleDic.TryGetValue(bundle_name, out bundle))
        {
            //Debug.Log("PATH:" + ROOT_PATH + "/" + bundle_name);
            bundle = AssetBundle.LoadFromFile(ROOT_PATH + "/" + bundle_name);
            //Debug.Log("bundle:" + bundle);
            _assetbundleDic.Add(bundle_name, bundle);
        }
#endregion

#region 获取包里面的资源
        int assetNameStart = path.LastIndexOf("/") + 1;
        int assetNameEnd = path.LastIndexOf(".");
        string assetName = path.Substring(assetNameStart, assetNameEnd - assetNameStart);
        //Debug.Log("assetName:" + assetName);
        T obj = bundle.LoadAsset(assetName) as T;
        return obj;
#endregion
    }


    /// <summary>
    /// 从ab包里面读取一个物体
    /// </summary>
    /// <param name="bundle_name"></param>
    /// <param name="path"></param>
    /// <param name="suffix"></param>
    /// <returns></returns>
    public static GameObject InstantiateGameObject(string bundle_name, string path, string suffix = ".prefab")
    {
        GameObject go = LoadRes<GameObject>(bundle_name, path + suffix);
        if (go != null)
            return GameObject.Instantiate(go);
        return null;
    }
}
