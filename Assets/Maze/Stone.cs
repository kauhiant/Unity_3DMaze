﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    public class Stone : MazeObject
    {
        public Stone(Point3D position) : base(position)
        {
        }

        public override Sprite GetSprite()
        {
            return GlobalAsset.stoneSprite;
        }
        
    }
}
