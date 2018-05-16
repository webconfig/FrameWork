using UnityEngine;
using System.Collections.Generic;

public class World
{
    public Vector3 start_positon;
    public int x_add;
    public int z_add;
    public int x_max;
    public int z_max;
    public Cell[,] items;
    
    public World(Vector3 p, int _x_add, int _z_add, int _x_max, int _z_max)
    {
        start_positon = p;

        x_add = _x_add;
        z_add = _z_add;
        x_max = _x_max;
        z_max = _z_max;
        items = new Cell[x_max, z_max];
        Vector3 k = new Vector3();
        k.y = 0;
        vertices = new Vector3[(x_max+1) * (z_max+1)];
        for (int i = 0; i <= x_max; i++)
        {
            for (int j = 0; j <= z_max; j++)
            {
                vertices[i * (x_max+1) + j] = new Vector3(start_positon.x + i * x_add, 0, start_positon.z + j * z_add);
                //Debug.Log("index;" + (i * x_max + j) + "--" + j+"--"+(start_positon.z + j * z_add));
                if (i != x_max&&j!=z_max)
                {
                    k.x = start_positon.x + i * x_add + x_add * 0.5f;
                    k.z = start_positon.z + j * z_add + z_add * 0.5f;
                    //==顶点索引
                    items[i, j] = new Cell(0, k,new List<int>() {
                        i * (x_max+1) + j, i * (x_max+1) + j+1,(i+1)* (x_max+1)+j+1,
                        (i+1)* (x_max+1)+j+1,(i+1)* (x_max+1)+j,i * (x_max+1) + j
                    });
                }
            }
        }
    }

    public void RayCast()
    {
        for (int i = 0; i < x_max; i++)
        {
            for (int j = 0; j < z_max; j++)
            {
                if (Physics.Raycast(items[i, j].position + Vector3.up * 10, Vector3.up * -1, 12))
                {
                    items[i, j].state = 1;
                }
                else
                {
                    items[i, j].state = 0;
                }
            }
        }
        DrawBox();
    }

    private MeshFilter mesh_filter_green, mesh_filter_red;
    private MeshRenderer mesh_render_green, mesh_render_red;
    private Vector3[] vertices;
    private bool _debug = false;
    public void InitDebug(Material mat_green, Material mat_red)
    {
        GameObject obj_green = new GameObject();
        obj_green.name = "obj_green";
        mesh_filter_green = obj_green.AddComponent<MeshFilter>();
        mesh_render_green = obj_green.AddComponent<MeshRenderer>();
        mesh_render_green.material = mat_green;
        //==
        GameObject obj_red = new GameObject();
        obj_red.name = "obj_red";
        mesh_filter_red = obj_red.AddComponent<MeshFilter>();
        mesh_render_red = obj_red.AddComponent<MeshRenderer>();
        mesh_render_red.material = mat_red;
        _debug = true;
    }

    private  void DrawBox()
    {
        if (!_debug) { return; }
        List<int> triangles_green = new List<int>();
        List<int> triangles_red = new List<int>();

        for (int i = 0; i < x_max; i++)
        {
            for (int j = 0; j < z_max; j++)
            {
                if (items[i, j].state == 1)
                {
                    triangles_red.AddRange(items[i, j].triangles);
                }
                else
                {
                    triangles_green.AddRange(items[i, j].triangles);
                }
            }
        }

        mesh_filter_green.mesh.Clear();
        mesh_filter_green.mesh.vertices = vertices;
        mesh_filter_green.mesh.triangles = triangles_green.ToArray();

        mesh_filter_red.mesh.Clear();
        mesh_filter_red.mesh.vertices = vertices;
        mesh_filter_red.mesh.triangles = triangles_red.ToArray();
    }

    //=======调试========
    public void OnGUI()
    {
        Vector3 p1 = new Vector3(), p2 = new Vector3();
        p1.x = start_positon.x;
        p1.y = start_positon.y;

        p2.x = start_positon.x + x_max * x_add;
        p2.y = start_positon.y;
        //==横线
        for (int i = 0; i <= z_max; i++)
        {
            p1.z = start_positon.x + i * z_add;
            p2.z = start_positon.z + i * z_add;
            UnityEngine.Debug.DrawLine(p1, p2, UnityEngine.Color.red);

        }

        p1.z = start_positon.z;
        p1.y = start_positon.y;

        p2.z = start_positon.z + z_max * z_add;
        p2.y = start_positon.y;
        //==纵线
        for (int i = 0; i <= x_max; i++)
        {
            p1.x = start_positon.x + i * z_add;
            p2.x = start_positon.z + i * z_add;
            UnityEngine.Debug.DrawLine(p1, p2, UnityEngine.Color.red);

        }
        //for (int i = 0; i < x_max; i++)
        //{
        //    for (int j = 0; j < z_max; j++)
        //    {
        //        Gizmos.DrawCube(items[i, j].position, new Vector3(x_add*0.5f,0.1f,z_add*0.5f));
        //    }
        //}

    }
}
public class Cell
{
    public int state=0;
    public Vector3 position;
    public List<int> ids;
    
    public Cell(int _state,Vector3 _postion)
    {
        state = _state;
        position = _postion;
    }

    //============调试====
    public List<int> triangles;
    public Cell(int _state, Vector3 _postion, List<int> _triangles)
    {
        state = _state;
        position = _postion;
        triangles = _triangles;
    }
}

