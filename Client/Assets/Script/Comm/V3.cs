using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public struct V3
{
    public int x;
    public int y;
    public int z;

    public void Clone(V3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }
}
