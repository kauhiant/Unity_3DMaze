using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    class Wall : MazeObject
    {
        public Wall(Point3D position) : base(position)
        {

        }

        public void beAttack()
        {
            GlobalAsset.map.GetAt(this.position).obj = null;
            RegisterEvent(ObjEvent.Destroy);
        }

        public override Sprite Shape()
        {
            return GlobalAsset.wall;
        }
    }
}
