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

    public enum ObjEvent
    {
        move,shape,plain,Destroy,Grow,None
    }

    public enum Skill
    {
        attack, straight, horizon, create
    }

    public class VectorConvert
    {
        // 順時針轉90度.
        static public Vector2D Rotate(Vector2D vector)
        {
            switch (vector)
            {
                case Vector2D.Up:
                    return Vector2D.Right;
                case Vector2D.Down:
                    return Vector2D.Left;
                case Vector2D.Left:
                    return Vector2D.Up;
                case Vector2D.Right:
                    return Vector2D.Down;
                default:
                    return Vector2D.Null;
            }
        }

        // 反方向.
        static public Vector2D Invert(Vector2D vector)
        {
            switch (vector)
            {
                case Vector2D.Up:
                    return Vector2D.Down;
                case Vector2D.Down:
                    return Vector2D.Up;
                case Vector2D.Left:
                    return Vector2D.Right;
                case Vector2D.Right:
                    return Vector2D.Left;
                default:
                    return Vector2D.Null;
            }
        }
    }
}
