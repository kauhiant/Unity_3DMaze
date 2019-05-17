using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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
        posit, rotate, scale, sprite, color, plain, damage, Destroy
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

        // 將 Maze 的方向轉成 Unity 的方向.
        public static Vector2 Convert(Vector2D vector)
        {
            switch (vector)
            {
                case Maze.Vector2D.Up:
                    return Vector2.up;
                case Maze.Vector2D.Down:
                    return Vector2.down;
                case Maze.Vector2D.Left:
                    return Vector2.left;
                case Maze.Vector2D.Right:
                    return Vector2.right;
                default:
                    return Vector2.zero;
            }
        }

        // 將 Maze 的座標轉成 Unity 的座標.
        public static Vector2 Convert(Point2D point)
        {
            return new Vector2(point.X.value, point.Y.value);
        }

    }
}
