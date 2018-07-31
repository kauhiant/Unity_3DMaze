using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    /// 負責 Maze.MazeObject 和 Unity.GameObject 之間的溝通.
    /// 如果 Unity 的 camera 可以自動跟隨 PlayerBind ，就不需要這裡的 camera.
    public class MapManager
    {
        class Pair
        {
            public GameObject binded;
            public MazeObject obj;
            public Pair(MazeObject obj, GameObject binded)
            {
                this.obj = obj;
                this.binded = binded;
            }
        }


        private List<GameObject> grids;
        private List<Pair> objs;
        
        private Map2D map;
        private GameObject camera;
        private Point2D bufferCenter;
        private int     bufferExtra;
        private bool isMove;

        private Animal Player { get { return GlobalAsset.player; } }
        private int Width { get { return bufferExtra * 2 + 1; } }

        public GameObject PlayerBind { get { return FindMazeObject(Player).binded; } }
        public bool GameOver
        {
            private set { GlobalAsset.gameOver = value; }
            get { return GlobalAsset.gameOver; }
        }


        
        public MapManager(Map2D map, GameObject camera, int extra)
        {
            this.map = map;
            this.camera = camera;
            this.bufferCenter = Player.PositOnScene.Copy();
            this.bufferExtra  = extra;
            this.isMove = false;

            this.grids = new List<GameObject>();
            this.objs  = new List<Pair>();

            GameOver = false;
            camera.transform.position = new Vector3(bufferCenter.X.value, bufferCenter.Y.value, -1);
            ShowMap();
        }

        /// <summary>
        /// run on a clock.
        /// </summary>
        public void UpdateScene()
        {
            if (GameOver) return;

            // update player
            UpdateObject(FindMazeObject(Player));

            // if player move
            if (isMove)
            {
                // change grids.
                MoveForward(Player.vectorOnScenen);
                isMove = false;
            }

            // change objs.
            RemoveObjOutBuffer();
            AddObjInBuffer();

            // update objs
            for (int i = 0; i < objs.Count; ++i)
            {
                UpdateObject(objs[i]);

                // if the obj is destroyed.
                if (objs[i].binded == null)
                {
                    if (objs[i].obj == Player)
                        GameOver = true;

                    objs.RemoveAt(i);
                    --i;
                    continue;
                }
                

                /// 目前只有 Animal 會一直改變顏色(alpha)，未來不一定.
                /// 可以把 if 判斷式拿掉，不過可能稍微耗效能.
                if (objs[i].obj is Animal)
                    ObjectChangeColor(objs[i]);
            }
        }

        /// <summary>
        /// use this function after player.changePlain.
        /// </summary>
        public void ChangePlain()
        {
            ClearMap();
            this.bufferCenter = Player.PositOnScene.Copy();
            camera.transform.position = new Vector3(bufferCenter.X.value, bufferCenter.Y.value, -1);
            ShowMap();
        }

        /// <summary>
        /// just change grids.
        /// </summary>
        /// <param name="vector"></param>
        private void MoveForward(Vector2D vector)
        {
            /// remove line.
            Dimention dimention = Dimention.Null;
            int value = 0;
            
            switch (vector)
            {
                case Vector2D.Up:
                    dimention = Dimention.Y;
                    value = bufferCenter.Y.value - bufferExtra;
                    break;
                case Vector2D.Down:
                    dimention = Dimention.Y;
                    value = bufferCenter.Y.value + bufferExtra;
                    break;
                case Vector2D.Left:
                    dimention = Dimention.X;
                    value = bufferCenter.X.value + bufferExtra;
                    break;
                case Vector2D.Right:
                    dimention = Dimention.X;
                    value = bufferCenter.X.value - bufferExtra;
                    break;
            }

            RemoveLine(dimention, value);


            /// move bufferCenter and camera.
            this.bufferCenter.MoveFor(vector, 1);
            this.camera.transform.Translate(ConvertTo(vector));


            /// add line
            Point2D point = this.bufferCenter.Copy();
            point.MoveFor(vector, bufferExtra);
            vector = VectorConvert.Rotate(vector);
            point.MoveFor(vector, bufferExtra);
            vector = VectorConvert.Invert(vector);

            AddLine(point, vector, Width);
        }
        


        private void CreateGridAt(int x, int y, Grid grid)
        {
            GameObject temp = new GameObject();
            temp.transform.position = new Vector3(x, y, 0);
            temp.AddComponent<SpriteRenderer>().sprite = grid.shape;
            temp.GetComponent<SpriteRenderer>().sortingLayerName = "grid";

            grids.Add(temp);
        }

        private void CreateObjAt(int x, int y, MazeObject obj)
        {
            GameObject temp = new GameObject();
            temp.transform.position = new Vector3(x, y, 0);
            temp.AddComponent<SpriteRenderer>().sprite = obj.GetSprite();
            temp.GetComponent<SpriteRenderer>().sortingLayerName = "object";

            obj.RegisterEvent(ObjEvent.None);
            objs.Add(new Pair(obj,temp));

            if(obj is Creater)
            {
                temp.GetComponent<SpriteRenderer>().sortingLayerName = "creater";
                temp.GetComponent<SpriteRenderer>().color = obj.GetColor();
                temp.transform.localScale = obj.GetScale();
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

        // just remove grids
        private void RemoveLine(Dimention dimention, int value)
        {
            int index = 0;
            while(index != grids.Count)
            {
                if (IsOnLine(grids[index], dimention, value))
                {
                    GameObject.Destroy(grids[index]);
                    grids.RemoveAt(index);
                }
                else
                    ++index;
            }
        }

        // just add grids
        private void AddLine(Point2D start, Vector2D vector, int dist)
        {
            while(dist > 0)
            {
                AddGridAt(start);
                start.MoveFor(vector, 1);
                --dist;
            }
        }



        private void ShowMap()
        {
            Iterator iter = new Iterator(bufferCenter, bufferExtra);

            do
            {
                Point2D point = iter.Iter;
                Grid grid = map.GetAt(point);

                if (grid != null)
                {
                    CreateGridAt(point.X.value, point.Y.value, grid);
                    if (grid.Obj != null)
                        CreateObjAt(point.X.value, point.Y.value, grid.Obj);
                }
            } while (iter.MoveToNext());
        }

        private void ClearMap()
        {
            while (grids.Count != 0)
            {
                GameObject.Destroy(grids[0]);
                grids.RemoveAt(0);
            }

            while (objs.Count != 0)
            {
                GameObject.Destroy(objs[0].binded);
                objs.RemoveAt(0);
            }
        }



        private Point2D CreatePoint(int x, int y)
        {
            Point2D point = this.bufferCenter.Copy();
            point.X.value = x;
            point.Y.value = y;
            return point;
        }

        private Vector2 ConvertTo(Maze.Vector2D vector)
        {
            switch (vector)
            {
                case Maze.Vector2D.Up:
                    return Vector2.up;
                case Maze.Vector2D.Down:
                    return Vector2.down;
                case Maze.Vector2D.Left:
                    return Vector2.left;
                case Maze.Vector2D.Right:
                    return Vector2.right;
                default:
                    return Vector2.zero;
            }
        }

        private Vector2 ConvertTo(Point2D point)
        {
            return new Vector2(point.X.value, point.Y.value);
        }

        private Point2D ConvertTo(Vector2 vector)
        {
            int x = (int)Mathf.Round(vector.x);
            int y = (int)Mathf.Round(vector.y);
            return CreatePoint(x, y);
        }



        private void UpdateObject(Pair objPair)
        {
            switch (objPair.obj.GetEvent())
            {
                case Maze.ObjEvent.move:
                    GameObjectMove(objPair);
                    break;
                    
                case Maze.ObjEvent.shape:
                    objPair.binded.GetComponent<SpriteRenderer>().sprite = objPair.obj.GetSprite();
                    break;

                case ObjEvent.Destroy:
                    GameObject.Destroy(objPair.binded);
                    objPair.binded = null;
                    break;

                case ObjEvent.Grow:
                    objPair.binded.transform.localScale = objPair.obj.GetScale();
                    break;

                case Maze.ObjEvent.None:
                    break;
            }
        }

        private void GameObjectMove(Pair objPair)
        {
            if(objPair.obj is Animal)
            {
                Animal animal = (Animal)objPair.obj;
                Vector2D vector = animal.vectorOnScenen;
                Vector2 vect = ConvertTo(vector);
                
                objPair.binded.transform.Translate(vect);

                if (objPair.obj == Player)
                    this.isMove = true;
            }
            else
            {
                Debug.Log("error: Maze.MapManager.GameObjectMove() -> gameObject is not Animal");
            }
        }

        private void GameObkectMove(GameObject gameObject, Vector2D vector)
        {
            gameObject.transform.Translate(ConvertTo(vector));
        }

        private void ObjectChangeColor(Pair objPair)
        {
            objPair.binded.GetComponent<SpriteRenderer>().color = objPair.obj.GetColor();
        }
        


        private Pair FindMazeObject(MazeObject mazeObject)
        {
            foreach(Pair each in objs)
            {
                if (each.obj == mazeObject)
                    return each;
            }
            return null;
        }

        private bool IsOutOfBuffer(MazeObject obj)
        {
            if (!obj.position.IsOnPlain(this.Player.plain))
                return true;

            Point2D position = obj.PositOnScene;
            return (
                position.X.value < bufferCenter.X.value - bufferExtra ||
                position.X.value > bufferCenter.X.value + bufferExtra ||
                position.Y.value < bufferCenter.Y.value - bufferExtra ||
                position.Y.value > bufferCenter.Y.value + bufferExtra);
        }

        private bool BufferHaveObj(MazeObject obj)
        {
            foreach(Pair each in objs)
            {
                if (each.obj == obj)
                    return true;
            }
            return false;
        }



        private void AddObjInBuffer()
        {
            Iterator iter = new Iterator(bufferCenter, bufferExtra);

            do
            {
                Point2D point = iter.Iter;
                Grid grid = map.GetAt(point);
                if (grid != null && grid.Obj != null)
                {
                    if (!BufferHaveObj(grid.Obj))
                        CreateObjAt(point.X.value, point.Y.value, grid.Obj);
                }
            } while (iter.MoveToNext());
        }

        private void RemoveObjOutBuffer()
        {
            int i = 0;
            while (i < objs.Count)
            {
                Pair each = objs[i];

                if (IsOutOfBuffer(each.obj))
                {
                    GameObject.Destroy(each.binded);
                    objs.RemoveAt(i);
                }
                else
                    ++i;
            }
        }
    }
}
