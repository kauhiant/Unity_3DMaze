using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    public class Point3D
    {
        public Int X { get; private set; }
        public Int Y { get; private set; }
        public Int Z { get; private set; }


        public Point3D(int x, int y, int z)
        {
            this.X = new Int(x);
            this.Y = new Int(y);
            this.Z = new Int(z);
        }

        public Point3D Copy() {
            return new Point3D(X.value, Y.value, Z.value);
        }


        public void Set(int x, int y, int z)
        {
            this.X.value = x;
            this.Y.value = y;
            this.Z.value = z;
        }

        public void SetBy(Point3D point)
        {
            this.X.value = point.X.value;
            this.Y.value = point.Y.value;
            this.Z.value = point.Z.value;
        }

        public void MoveFor(Vector3D vector, int dist) {
            switch (vector)
            {
                case Vector3D.Xn:
                    this.X.Add(-dist);
                    break;

                case Vector3D.Xp:
                    this.X.Add(dist);
                    break;

                case Vector3D.Yn:
                    this.Y.Add(-dist);
                    break;

                case Vector3D.Yp:
                    this.Y.Add(dist);
                    break;

                case Vector3D.Zn:
                    this.Z.Add(-dist);
                    break;

                case Vector3D.Zp:
                    this.Z.Add(dist);
                    break;
            }
        }


        public bool IsOnPlain(Plain plain)
        {
            switch (plain.Dimention)
            {
                case Dimention.X:
                    return this.X.value == plain.Value;
                case Dimention.Y:
                    return this.Y.value == plain.Value;
                case Dimention.Z:
                    return this.Z.value == plain.Value;
                default:
                    return false;
            }
        }

        public bool IsOnRange(Range2D range)
        {
            Point2D point = new Point2D(this, range.Center.Plain.Dimention);

            if (!point.IsOnPlain(range.Center.Plain))
                return false;

            return Math.Abs(point.X.value - range.Center.X.value) <= range.XWidth / 2
                && Math.Abs(point.Y.value - range.Center.Y.value) <= range.YWidth / 2;
        }

        public bool Equals(Point3D point)
        {
            return (this.X.value == point.X.value &&
                    this.Y.value == point.Y.value &&
                    this.Z.value == point.Z.value);
        }


        public override string ToString()
        {
            return string.Format("({0},{1},{2})", X.value, Y.value, Z.value);
        }
    }
}
