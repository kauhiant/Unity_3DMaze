using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    class Food:MazeObject
    {
        public int nutrient;

        public Food(Point3D position, int nutrient):base(position)
        {
            this.nutrient = nutrient;
        }

        public override Sprite Shape()
        {
            return GlobalAsset.food;
        }
    }
}
