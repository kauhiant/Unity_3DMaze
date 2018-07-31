using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    class Wall : MazeObject,Attackable
    {
        public Wall(Point3D position) : base(position)
        {

        }

        public void BeAttack(Animal animal)
        {
            GlobalAsset.map.GetAt(this.position).RemoveObj();
            RegisterEvent(ObjEvent.Destroy);
        }

        public override Sprite Shape()
        {
            return GlobalAsset.wall;
        }
    }
}
