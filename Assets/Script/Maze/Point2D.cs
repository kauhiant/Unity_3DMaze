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
    // 要與一個 Point3D 綁定.
    // 負責把 3D 轉 2D .
    // 用 2D 的介面控制.
    public class Point2D
    {
        public Int X { get; private set; }
        public Int Y { get; private set; }
        public Point3D Binded { get; private set; }
        public Dimention Dimention { get; private set; }

        public Plain Plain // 此 Point2D 所在的平面
        {
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
        

        // 與 Point3D 綁定，以 dimention 設定平面.
        public void Bind(Point3D binded, Dimention dimention)
        {
            this.Binded = binded;
            this.ChangePlain(dimention);
        }

        // 以 dimention 設定平面.
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

        // 向 vector 移動 distance.
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
        
        // 是否在同一平面.
        public bool IsOnPlain(Plain plain)
        {
            return this.Binded.IsOnPlain(plain);
        }

        // 傳回最長直線距離.
        // 如果不再同一平面，回傳最大值.
        public int DistanceTo(Point3D point)
        {
            if (!point.IsOnPlain(this.Plain))
                return int.MaxValue;

            Point2D pointOnSamePlain = new Point2D(point.Copy(), this.Plain.Dimention);

            int xDist = Math.Abs(this.X.value - pointOnSamePlain.X.value);
            int yDist = Math.Abs(this.Y.value - pointOnSamePlain.Y.value);

            return (xDist > yDist) ? xDist : yDist;
        }

        // 只看 x,y ，不看 binded.
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
