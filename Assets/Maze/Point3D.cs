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
    }
}
