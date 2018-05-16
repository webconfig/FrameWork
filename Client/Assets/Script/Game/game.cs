
using UnityEngine;

public class Game
{
    public string IP = "192.168.2.90";
    public int Port = 3333;
    public World world;
    //==网络
    public NetWorkManager network;
    public void Init()
    {
        //==网络
        network = new NetWorkManager();
        network.Init(IP, Port);
        network.Conn(1);

        //==初始化地图
        world = new World(new Vector3(0, 0, 0), 1, 1,3, 3);
    }

   

    //void DrawSquare()
    //{
    //    gameObject.AddComponent<MeshFilter>();
    //    gameObject.AddComponent<MeshRenderer>();
    //    gameObject.GetComponent<MeshRenderer>().material = mat;

    //    Mesh mesh = GetComponent<MeshFilter>().mesh;
    //    mesh.Clear();

    //    mesh.vertices = new Vector3[] { new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 0, 0) };
    //    mesh.triangles = new int[]
    //    { 0, 1, 2,
    //      0, 2, 3
    //    };
    //}

}


