using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    public class Stone : MazeObject
    {
        static private Sprite Sprite;
        static public void SetSprite(Sprite sprite)
        {
            Stone.Sprite = sprite;
        }

        public Stone(Point3D position) : base(position)
        {
        }

        public override Sprite GetSprite()
        {
            return Stone.Sprite;
        }
        
    }
}
