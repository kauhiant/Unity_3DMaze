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

        public Animal(Point3D position) : base(position)
        {
            posit = new Point2D(this.position, Dimention.Z);
        }

        public bool MoveFor(Vector2D vector)
        {

            Point2D temp = this.posit.Copy();
            temp.MoveFor(vector, 1);
            Grid targetGrid = GlobalAsset.map.GetAt(temp.binded);

            if (targetGrid == null)
            {
                return false;
            }
                

            if (targetGrid.obj != null)
            {
                return false;
            }
                

            GlobalAsset.map.Swap(position, temp.binded);
            
            return true;
            
        }

        public void ChangePlain(Dimention dimen)
        {
            this.posit.ChangePlain(dimen);
        }

        public override Sprite Shape()
        {
            return GlobalAsset.animalSprite;
        }
    }
}
