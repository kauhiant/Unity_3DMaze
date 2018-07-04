using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    // 三個軸向.
    public enum Dimention
    {
        X, Y, Z, Null
    }

    // 以2D視角來看，有6個方向.
    public enum Vector2D
    {
        Up, Down, Left, Right, In, Out, Null
    }

    // X,Y,Z 三個軸.
    // p : 正向.
    // n : 負向.
    public enum Vector3D
    {
        Xp, Xn, Yp, Yn, Zp, Zn, Null
    }
}
