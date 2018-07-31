using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    public class Map2D
    {
        public Map3D Binded { get; private set; }

        public Map2D(Map3D binded)
        {
            this.Binded = binded;
        }

        public Grid GetAt(Point2D position)
        {
            return Binded.GetAt(position.Binded);
        }
    }
}
