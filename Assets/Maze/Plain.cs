using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    public class Plain
    {
        public Dimention Dimention { get; private set; }
        public int Value { get; private set; }


        public Plain(Point3D point, Dimention dimention)
        {
            this.Dimention = dimention;
            switch (dimention)
            {
                case Dimention.X:
                    this.Value = point.X.value;
                    break;

                case Dimention.Y:
                    this.Value = point.Y.value;
                    break;

                case Dimention.Z:
                    this.Value = point.Z.value;
                    break;
            }
        }


        public bool IsEqual(Plain plain)
        {
            return this.Dimention == plain.Dimention
                && this.Value == plain.Value;
        }
        

        public Vector2D Vector3To2(Vector3D vector)
        {
            switch (Dimention)
            {
                case Dimention.X:
                    switch (vector)
                    {
                        case Vector3D.Xp: return Vector2D.Out;
                        case Vector3D.Xn: return Vector2D.In;
                        case Vector3D.Yp: return Vector2D.Right;
                        case Vector3D.Yn: return Vector2D.Left;
                        case Vector3D.Zp: return Vector2D.Up;
                        case Vector3D.Zn: return Vector2D.Down;
                    }break;

                case Dimention.Y:
                    switch (vector)
                    {
                        case Vector3D.Xp: return Vector2D.Up;
                        case Vector3D.Xn: return Vector2D.Down;
                        case Vector3D.Yp: return Vector2D.Out;
                        case Vector3D.Yn: return Vector2D.In;
                        case Vector3D.Zp: return Vector2D.Right;
                        case Vector3D.Zn: return Vector2D.Left;
                    }break;

                case Dimention.Z:
                    switch (vector)
                    {
                        case Vector3D.Xp: return Vector2D.Right;
                        case Vector3D.Xn: return Vector2D.Left;
                        case Vector3D.Yp: return Vector2D.Up;
                        case Vector3D.Yn: return Vector2D.Down;
                        case Vector3D.Zp: return Vector2D.Out;
                        case Vector3D.Zn: return Vector2D.In;
                    }break;
            }
            return Vector2D.Null;
        }

        public Vector3D Vector2To3(Vector2D vector)
        {
            switch (Dimention)
            {
                case Dimention.X:
                    switch (vector)
                    {
                        case Vector2D.Up:    return Vector3D.Zp;
                        case Vector2D.Down:  return Vector3D.Zn;
                        case Vector2D.Left:  return Vector3D.Yn;
                        case Vector2D.Right: return Vector3D.Yp;
                        case Vector2D.In:    return Vector3D.Xn;
                        case Vector2D.Out:   return Vector3D.Xp;
                    }break;

                case Dimention.Y:
                    switch (vector)
                    {
                        case Vector2D.Up:    return Vector3D.Xp;
                        case Vector2D.Down:  return Vector3D.Xn;
                        case Vector2D.Left:  return Vector3D.Zn;
                        case Vector2D.Right: return Vector3D.Zp;
                        case Vector2D.In:    return Vector3D.Yn;
                        case Vector2D.Out:   return Vector3D.Yp;
                    }break;

                case Dimention.Z:
                    switch (vector)
                    {
                        case Vector2D.Up:    return Vector3D.Yp;
                        case Vector2D.Down:  return Vector3D.Yn;
                        case Vector2D.Left:  return Vector3D.Xn;
                        case Vector2D.Right: return Vector3D.Xp;
                        case Vector2D.In:    return Vector3D.Zn;
                        case Vector2D.Out:   return Vector3D.Zp;
                    }break;
            }

            return Vector3D.Null;
        }
    }
}
