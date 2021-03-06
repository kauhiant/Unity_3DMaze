﻿using System;
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
        // 此遊戲的物件都會創造在這個世界.
        static public Map3D World { get; private set; }
        static public void SetMaze(Map3D world)
        {
            MazeObject.World = world;
        }

        private List<ObjEvent> objEvents; // 物件事件，給 Mapmanager 更新綁定物件的資料.
        
        public Point3D position;    // 在地圖上的位置.
        public Point2D PositOnScene // 在 Player 視角上的位置，提供給 MapManager 讓他創造更新綁定的 GameObject.
        { get { return new Point2D(this.position, GlobalAsset.player.Plain.Dimention); } }



        public MazeObject(Point3D position)
        {
            this.position = position;
            this.objEvents = new List<ObjEvent>();
        }



        // 以下3個都是提供給 MapManager 讓他創造更新綁定的 GameObject 用的.
        public abstract Sprite  GetSprite();
        public virtual  Color   GetColor ()
        {
            return Color.white;
        }
        public virtual  Vector2 GetScale ()
        {
            return Vector2.one;
        }
        

        // 將他從 World 上移除，並登記 Destroy 事件，提醒 MapManager 將它消除.
        public virtual void Destroy()
        {
            this.RegisterEvent(ObjEvent.Destroy);
            World.GetAt(this.position).RemoveObj();
        }

        // 登記事件，提醒 MapManager 對他的綁定物件進行動作.
        public void RegisterEvent(ObjEvent eventName)
        {
            this.objEvents.Add(eventName);
        }

        // 清空事件，MapManager 創造綁定物件時使用.
        public void InitEvents()
        {
            this.objEvents.Clear();
        }

        // 取得事件，讓 MapManager 知道該對他的綁定物件進行什麼動作.
        public List<ObjEvent> GetEvents()
        {
            return objEvents;
        }


        // 滑鼠移入顯示訊息
        public virtual String Name()
        {
            return "MazeObject";
        }

        public virtual String Info()
        {
            return "";
        }
    }

}
