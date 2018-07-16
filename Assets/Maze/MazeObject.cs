using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{

    abstract public class MazeObject
    {
        public Point3D position;

        public MazeObject(Point3D position)
        {
            this.position = position;
        }

        public abstract Sprite Shape();
    }

}
