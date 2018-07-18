using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    public class Point2D
    {
        public Point3D   binded;
        public Dimention dimen;
        public Int       x, y;

        public Plain plain { get { return new Plain(binded, dimen); } }

        public Point2D(Point3D binded, Dimention dimen)
        {
            this.Bind(binded, dimen);
        }

        public Point2D Copy()
        {
            return new Point2D(binded.Copy(), dimen);
        }

        public void Bind(Point3D binded)
        {
            this.Bind(binded, this.dimen);
        }

        public void Bind(Point3D binded, Dimention dimen)
        {
            this.binded = binded;
            this.ChangePlain(dimen);
        }
        
        // X : ( Y,Z )
        // Y : ( Z,X )
        // Z : ( X,Y )
        public void ChangePlain(Dimention dimen)
        {
            this.dimen = dimen;

            switch (dimen)
            {
                case Dimention.X:
                    this.x = binded.y;
                    this.y = binded.z;
                    break;

                case Dimention.Y:
                    this.x = binded.z;
                    this.y = binded.x;
                    break;

                case Dimention.Z:
                    this.x = binded.x;
                    this.y = binded.y;
                    break;
            }
        }

        public void MoveFor(Vector2D vector, int distance)
        {
            switch (vector)
            {
                case Vector2D.Up:
                    this.y.Add(1 * distance);
                    break;

                case Vector2D.Down:
                    this.y.Add(-1 * distance);
                    break;

                case Vector2D.Left:
                    this.x.Add(-1 * distance);
                    break;

                case Vector2D.Right:
                    this.x.Add(1 * distance);
                    break;
            }
        }

        
    }
}
