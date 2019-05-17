using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    class BufferMap
    {
        private Animal Player
        { get { return GlobalAsset.player; } }
        public BindObject PlayerBind { get; private set; }

        // RealMap
        private Map2D map;

        // GameObjects and MazeObjects
        private List<BindGrid> grids;
        private List<BindObject> objs;

        public Point2D center;
        private int extra;
        private int Width
        { get { return extra * 2 + 1; } }


        public BufferMap(Map2D map, Point2D center, int extra)
        {
            this.grids = new List<BindGrid>();
            this.objs = new List<BindObject>();
            this.map = map;

            this.center = center;
            this.extra = extra;
        }


        // 創造 buffer 的 GameObject.
        // Grids and MazeObjects.
        public void ShowMap()
        {
            Iterator iter = new Iterator(center, extra);

            do
            {
                Point2D point = iter.Iter;
                Grid grid = map.GetAt(point);

                if (grid != null)
                {
                    CreateGridAt(point.X.value, point.Y.value, grid);

                    if (grid.Obj != null)
                    {
                        if(grid.Obj == Player)
                            PlayerBind = CreateObj(grid.Obj); 
                        else
                            CreateObj(grid.Obj);
                    }
                        
                }
            } while (iter.MoveToNext());
        }

        // 將場上的 GameObject 清空.
        // 場上此管理器創造出來的.
        public void ClearMap()
        {
            while (grids.Count != 0)
            {
                grids[0].Destroy();
                grids.RemoveAt(0);
            }

            while (objs.Count != 0)
            {
                objs[0].Destroy();
                objs.RemoveAt(0);
            }
        }

        // 當 player 換人時，要呼叫此方法.
        // 不然上一個玩家的視角會殘留.
        // 當 player 改變視角時，要呼叫此方法.
        // 不然 buffer 會看到其他地方.
        public void ReBindPlayer()
        {
            ClearMap();
            this.center = Player.PositOnScene.Copy();
            ShowMap();
        }
        
        // test
        public void Clock()
        {
            // change objs.
            RemoveObjOutBuffer();
            AddObjInBuffer();

            // update objs
            for (int i = 0; i < objs.Count; ++i)
            {
                objs[i].UpdateBinded();

                // if the obj is destroyed.
                if (objs[i].binded == null)
                {
                    objs.RemoveAt(i);
                    --i;
                    continue;
                }
            }
        }


        // 創造 grid 的綁定物件.
        private BindGrid CreateGridAt(int x, int y, Grid grid)
        {
            BindGrid bindGrid = new BindGrid(grid, x, y);
            grids.Add(bindGrid);
            return bindGrid;
        }

        // 創造 MazeObject 的綁定物件.
        private BindObject CreateObj(MazeObject obj)
        {
            BindObject bindObject = new BindObject(obj);
            objs.Add(bindObject);
            obj.InitEvents();
            return bindObject;
        }

        // 將 buffer 往 vector 移動一格.
        // camera 也會更著移動.
        public void MoveForward(Vector2D vector)
        {
            // remove line.
            Dimention dimention = Dimention.Null;
            int value = 0;

            switch (vector)
            {
                case Vector2D.Up:
                    dimention = Dimention.Y;
                    value = center.Y.value - extra;
                    break;
                case Vector2D.Down:
                    dimention = Dimention.Y;
                    value = center.Y.value + extra;
                    break;
                case Vector2D.Left:
                    dimention = Dimention.X;
                    value = center.X.value + extra;
                    break;
                case Vector2D.Right:
                    dimention = Dimention.X;
                    value = center.X.value - extra;
                    break;
            }

            RemoveLine(dimention, value);


            // move bufferCenter and camera.
            this.center.MoveFor(vector, 1);
    //        GameObjectMove(camera, Convert(vector));


            // add line.
            Point2D point = this.center.Copy();
            point.MoveFor(vector, extra);
            vector = VectorConvert.Rotate(vector);
            point.MoveFor(vector, extra);
            vector = VectorConvert.Invert(vector);

            AddLine(point, vector, Width);
        }


        // just remove grids
        private void RemoveLine(Dimention dimention, int value)
        {
            int index = 0;
            while (index != grids.Count)
            {
                if (IsOnLine(grids[index].binded, dimention, value))
                {
                    grids[index].Destroy();
                    grids.RemoveAt(index);
                }
                else
                    ++index;
            }
        }
        
        // just add grids
        private void AddLine(Point2D start, Vector2D vector, int dist)
        {
            while (dist > 0)
            {
                AddGridAt(start);
                start.MoveFor(vector, 1);
                --dist;
            }
        }

        // obj is on the line(dimention:value) ?
        private bool IsOnLine(GameObject obj, Dimention dimention, int value)
        {
            float dimen = 0;
            switch (dimention)
            {
                case Dimention.X:
                    dimen = obj.transform.position.x;
                    break;
                case Dimention.Y:
                    dimen = obj.transform.position.y;
                    break;
                case Dimention.Z:
                    dimen = obj.transform.position.z;
                    break;
            }

            return Math.Round(dimen) == value;
        }

        // just add grid
        private void AddGridAt(Point2D point)
        {
            Grid grid = map.GetAt(point);
            if (grid != null)
            {
                CreateGridAt(point.X.value, point.Y.value, grid);
            }
        }



        // Clock : 把 buffer 內未被加入的 MazeObject 加入場上.
        // 因為 MazeObject 會移動、會死亡、會生成，不向 Grid 是固定的.
        // 所以每段時間都要更新一次.
        private void AddObjInBuffer()
        {
            Iterator iter = new Iterator(center, extra);

            do
            {
                Point2D point = iter.Iter;
                Grid grid = map.GetAt(point);
                if (grid != null && grid.Obj != null)
                {
                    if (!BufferHaveObj(grid.Obj))
                        CreateObj(grid.Obj);
                }
            } while (iter.MoveToNext());
        }

        // Clock : 把超出 buffer 的 MazeObject 從場上移除.
        // 因為 MazeObject 會移動、會死亡、會生成，不像 Grid 是固定的.
        // 所以每段時間都要更新一次.
        private void RemoveObjOutBuffer()
        {
            int i = 0;
            while (i < objs.Count)
            {
                var each = objs[i];

                if (IsOutOfBuffer(each.obj))
                {
                    each.Destroy();
                    objs.RemoveAt(i);
                }
                else
                    ++i;
            }
        }

        

        // obj 的位置是否在 buffer 外?
        private bool IsOutOfBuffer(MazeObject obj)
        {
            if (!obj.position.IsOnPlain(GlobalAsset.player.Plain))
                return true;

            Point2D position = obj.PositOnScene;
            return (
                position.X.value < center.X.value - extra ||
                position.X.value > center.X.value + extra ||
                position.Y.value < center.Y.value - extra ||
                position.Y.value > center.Y.value + extra);
        }

        // obj 是否已存在場上的 buffer ?
        private bool BufferHaveObj(MazeObject obj)
        {
            foreach (var each in objs)
            {
                if (each.obj == obj)
                    return true;
            }
            return false;
        }
    }
}
