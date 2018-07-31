using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    class Food:MazeObject
    {
        public int Nutrient { get; private set; }

        public Food(Point3D position, int nutrient):base(position)
        {
            this.Nutrient = nutrient;
        }

        public override Sprite Shape()
        {
            return GlobalAsset.food;
        }
    }
}
