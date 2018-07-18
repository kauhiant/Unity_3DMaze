using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    class Animal : MazeObject
    {
        public Point2D posit;
        public Vector2D vector;

        public Animal(Point3D position) : base(position)
        {
            posit = new Point2D(this.position, Dimention.Z);
            vector = Vector2D.Out;
        }

        public void MoveFor(Vector2D vector)
        {
            if (this.vector == vector)
                Move();
            else
                TurnTo(vector);
        }

        public void TurnTo(Vector2D vector)
        {
            this.vector = vector;
            RegisterEvent("turnTo");
        }

        public void Move()
        {
            Point2D temp = this.posit.Copy();
            temp.MoveFor(vector, 1);
            Grid targetGrid = GlobalAsset.map.GetAt(temp.binded);

            if (targetGrid == null)
            {
                return;
            }


            if (targetGrid.obj != null)
            {
                return;
            }

            RegisterEvent("move");
            GlobalAsset.map.Swap(position, temp.binded);
            return;
        }

        public void ChangePlain(Dimention dimen)
        {
            this.posit.ChangePlain(dimen);
        }

        public override Sprite Shape()
        {
            return GlobalAsset.anamalShape.At(vector);
        }

        private void RegisterEvent(string eventName)
        {
            GlobalAsset.map.GetAt(position).objEvent = eventName;
        }
    }
}
