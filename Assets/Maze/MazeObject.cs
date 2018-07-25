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
        public Point2D positOnScene
        { get { return new Point2D(this.position, GlobalAsset.player.plain.dimen); } }

        private ObjEvent objEvent;

        public MazeObject(Point3D position)
        {
            this.position = position;
            this.objEvent = ObjEvent.None;
        }

        public abstract Sprite Shape();

        public void RegisterEvent(ObjEvent eventName)
        {
            this.objEvent = eventName;
        }

        public ObjEvent GetEvent()
        {
            ObjEvent ret = this.objEvent;
            this.objEvent = ObjEvent.None;
            return ret;
        }
    }

}
