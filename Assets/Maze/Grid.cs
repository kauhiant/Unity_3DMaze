using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    public class Grid
    {
        public MazeObject Obj { get; private set; }
        public UnityEngine.Sprite shape;

        public bool IsEmpty()
        {
            return this.Obj == null;
        }

        public bool InsertObj(MazeObject obj)
        {
            if (this.Obj == null)
            {
                this.Obj = obj;
                return true;
            }
            else
                return false;
        }

        public MazeObject TakeOutObj()
        {
            MazeObject ret = this.Obj;
            this.Obj = null;
            return ret;
        }

        public void RemoveObj()
        {
            this.Obj = null;
        }

    }
}




