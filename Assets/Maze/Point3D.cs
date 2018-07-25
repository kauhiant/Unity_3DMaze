using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    public class Point3D
    {
        public Int x, y, z;

        public Point3D(int x, int y, int z)
        {
            this.x = new Int(x);
            this.y = new Int(y);
            this.z = new Int(z);
        }

        public Point3D Copy() {
            return new Point3D(x.value, y.value, z.value);
        }

        public void Set(int x, int y, int z)
        {
            this.x.value = x;
            this.y.value = y;
            this.z.value = z;
        }

        public void SetBy(Point3D point)
        {
            this.x.value = point.x.value;
            this.y.value = point.y.value;
            this.z.value = point.z.value;
        }

        public void MoveFor(Vector3D vector, int dist) {
            switch (vector)
            {
                case Vector3D.Xn:
                    this.x.Add(-dist);
                    break;

                case Vector3D.Xp:
                    this.x.Add(dist);
                    break;

                case Vector3D.Yn:
                    this.y.Add(-dist);
                    break;

                case Vector3D.Yp:
                    this.y.Add(dist);
                    break;

                case Vector3D.Zn:
                    this.z.Add(-dist);
                    break;

                case Vector3D.Zp:
                    this.z.Add(dist);
                    break;
            }
        }

        public bool isOnPlain(Plain plain)
        {
            switch (plain.dimen)
            {
                case Dimention.X:
                    return this.x.value == plain.value;
                case Dimention.Y:
                    return this.y.value == plain.value;
                case Dimention.Z:
                    return this.z.value == plain.value;
                default:
                    return false;
            }
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", x.value, y.value, z.value);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Point3D)) return false;
            Point3D point = (Point3D)obj;
            return (this.x.value == point.x.value &&
                this.y.value == point.y.value &&
                this.z.value == point.z.value);
        }
    }
}
