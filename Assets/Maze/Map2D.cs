using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    class Map2D
    {
        private Map3D binded;

        public Map2D(Map3D binded)
        {
            this.binded = binded;
        }

        public Grid GetAt(Point2D position)
        {
            return binded.GetAt(position.binded);
        }


    }
}
