using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    // 一個可以容納一個物件的格子.
    // 這個格子也會有自己的外觀，用來顯示在畫面上.
    public class Grid
    {
        // 這個格子顯示在畫面上出來的圖片.
        static public Sprite Sprite { get; private set; }
        static public void SetSprite(Sprite sprite)
        {
            Grid.Sprite = sprite;
        }

        // 在這個格子上的物件.
        public MazeObject Obj { get; private set; }


        // 這個格子沒放東西?
        public bool IsEmpty()
        {
            return this.Obj == null;
        }

        // 把物件放入這個格子內.
        // false : 格子內已經有東西了.
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

        // 把格子內的東西拿出來.
        // 回傳拿出來的物件.
        public MazeObject TakeOutObj()
        {
            MazeObject ret = this.Obj;
            this.Obj = null;
            return ret;
        }

        // 把格子內的東西移除丟掉.
        // 回傳此格子.
        public Grid RemoveObj()
        {
            this.Obj = null;
            return this;
        }

    }
}




