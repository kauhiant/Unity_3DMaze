using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    public class Plain
    {
        public Dimention dimen;
        public int value;

        public Plain(Point3D point, Dimention dimen)
        {
            this.dimen = dimen;
            switch (dimen)
            {
                case Dimention.X:
                    this.value = point.x.value;
                    break;

                case Dimention.Y:
                    this.value = point.y.value;
                    break;

                case Dimention.Z:
                    this.value = point.z.value;
                    break;
            }
        }

        public bool IsEqual(Plain plain)
        {
            return this.dimen == plain.dimen && this.value == plain.value;
        }

        public Vector2D Vector3To2(Vector3D vector)
        {
            switch (dimen)
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
            switch (dimen)
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
