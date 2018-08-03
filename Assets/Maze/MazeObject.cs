using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    // 可以放進格子的物件.
    // 也會顯示在畫面上.
    abstract public class MazeObject
    {
        static public Map3D World { get; private set; }
        static public void SetMaze(Map3D world)
        {
            MazeObject.World = world;
        }

        private ObjEvent objEvent; // 物件事件，給 Mapmanager 更新綁定物件的資料.
        
        public Point3D position;    // 在地圖上的位置.
        public Point2D PositOnScene // 在 Player 視角上的位置.
        { get { return new Point2D(this.position, GlobalAsset.player.Plain.Dimention); } }


        public MazeObject(Point3D position)
        {
            this.position = position;
            this.objEvent = ObjEvent.None;

            /* 之後要做的.
            if (!World.GetAt(position).InsertObj(this))
                Debug.Log(string.Format("{0} this grid is not empty, can insert Object",position));*/
        }

        // 以下3個都是提供給 MapManager 讓他創造更新綁定的 GameObject 用的.
        public abstract Sprite GetSprite();
        public virtual  Color GetColor()
        {
            return Color.white;
        }
        public virtual  Vector2 GetScale()
        {
            return Vector2.one;
        }
        
        // 將他從地圖上移除，並登記為Destroy物件，提醒 MapManager 將它消除.
        public virtual void Destroy()
        {
            this.RegisterEvent(ObjEvent.Destroy);
            World.GetAt(this.position).RemoveObj();
        }

        // 登記事件，提醒 MapManager 對他的綁定物件進行動作.
        public void RegisterEvent(ObjEvent eventName)
        {
            this.objEvent = eventName;
        }

        // 取得事件，讓 MapManager 知道該對他的綁定物件進行什麼動作.
        public ObjEvent GetEvent()
        {
            ObjEvent ret = this.objEvent;
            this.objEvent = ObjEvent.None;
            return ret;
        }
    }

}
