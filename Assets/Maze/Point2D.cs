using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    // binded : (X,Y,Z) 
    // dimention : ( x,y )
    // X         : ( Y,Z )
    // Y         : ( Z,X )
    // Z         : ( X,Y )
    public class Point2D
    {
        public Int X { get; private set; }
        public Int Y { get; private set; }
        public Point3D Binded { get; private set; }
        public Dimention Dimention { get; private set; }

        public Plain Plain{
            get{
                return new Plain(Binded, Dimention);
            }
        }


        public Point2D(Point3D binded, Dimention dimention)
        {
            this.Bind(binded, dimention);
        }

        public Point2D Copy()
        {
            return new Point2D(Binded.Copy(), Dimention);
        }
        

        public void Bind(Point3D binded, Dimention dimention)
        {
            this.Binded = binded;
            this.ChangePlain(dimention);
        }
        
        public void ChangePlain(Dimention dimention)
        {
            this.Dimention = dimention;

            switch (dimention)
            {
                case Dimention.X:
                    this.X = Binded.Y;
                    this.Y = Binded.Z;
                    break;

                case Dimention.Y:
                    this.X = Binded.Z;
                    this.Y = Binded.X;
                    break;

                case Dimention.Z:
                    this.X = Binded.X;
                    this.Y = Binded.Y;
                    break;
            }
        }

        public void MoveFor(Vector2D vector, int distance)
        {
            switch (vector)
            {
                case Vector2D.Up:
                    this.Y.Add(1 * distance);
                    break;

                case Vector2D.Down:
                    this.Y.Add(-1 * distance);
                    break;

                case Vector2D.Left:
                    this.X.Add(-1 * distance);
                    break;

                case Vector2D.Right:
                    this.X.Add(1 * distance);
                    break;
            }
        }

        /// <summary>
        /// is this.binded on the plain ?
        /// </summary>
        /// <param name="plain"></param>
        /// <returns></returns>
        public bool IsOnPlain(Plain plain)
        {
            return this.Binded.IsOnPlain(plain);
        }

        /// <summary>
        /// return far distance on Plain
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public int DistanceTo(Point3D point)
        {
            if (!point.IsOnPlain(this.Plain))
                return int.MaxValue;

            Point2D pointOnSamePlain = new Point2D(point.Copy(), this.Plain.Dimention);

            int xDist = Math.Abs(this.X.value - pointOnSamePlain.X.value);
            int yDist = Math.Abs(this.Y.value - pointOnSamePlain.Y.value);

            return (xDist > yDist) ? xDist : yDist;
        }

        public bool Equals(Point2D point)
        {
            return (point.X.value == this.X.value 
                 && point.Y.value == this.Y.value);
        }


        public override string ToString()
        {
            return string.Format("({0},{1})", X.value, Y.value);
        }
    }
}
