using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    abstract public class MazeObject
    {
        private ObjEvent objEvent;

        public Point3D position;
        public Point2D PositOnScene
        { get { return new Point2D(this.position, GlobalAsset.player.plain.Dimention); } }


        public MazeObject(Point3D position)
        {
            this.position = position;
            this.objEvent = ObjEvent.None;
        }

        public abstract Sprite GetSprite();
        public virtual  Color GetColor()
        {
            return Color.white;
        }
        public virtual  Vector2 GetScale()
        {
            return Vector2.one;
        }

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
