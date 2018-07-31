using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    public class MapManager
    {
        public class Pair
        {
            public GameObject binded;
            public MazeObject obj;
            public Pair(MazeObject obj, GameObject binded)
            {
                this.obj = obj;
                this.binded = binded;
            }
        }

        Animal     player;
        GameObject camera;

        List<GameObject> grids;
        List<Pair> objs;

        Map2D map;
        Point2D center;
        int extra;
        int width { get { return extra * 2 + 1; } }

        private bool isOnLine(GameObject obj, Dimention dimention, int value)
        {
            float dimen=0;
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

            return (value > dimen - 0.1f && value < dimen + 0.1f);
        }

        private void CreateObjAt(int x, int y, Maze.MazeObject obj)
        {
            GameObject temp = new GameObject();
            temp.transform.position = new Vector3(x, y, 0);
            temp.AddComponent<SpriteRenderer>();
            temp.GetComponent<SpriteRenderer>().sprite = obj.Shape();
            temp.GetComponent<SpriteRenderer>().sortingLayerName = "object";

            obj.RegisterEvent(ObjEvent.None);
            objs.Add(new Pair(obj,temp));

            if(obj is Creater)
            {
                temp.GetComponent<SpriteRenderer>().sortingLayerName = "creater";
                temp.GetComponent<SpriteRenderer>().color = ((Creater)obj).color;
                float scale = ((Creater)obj).getLevel() / 10f + 1;
                temp.transform.localScale = new Vector2(scale, scale);
            }

        }

        private void CreateGridAt(int x, int y, Maze.Grid grid)
        {
            GameObject temp = new GameObject();
            temp.transform.position = new Vector3(x, y, 0);
            temp.AddComponent<SpriteRenderer>();
            temp.GetComponent<SpriteRenderer>().sprite = grid.shape;
            temp.GetComponent<SpriteRenderer>().sortingLayerName = "grid";
            grids.Add(temp);
        }

        // just add grid
        private void addGridAt(Point2D point)
        {
            Grid grid = map.GetAt(point);
            if (grid != null)
            {
                CreateGridAt(point.X.value, point.Y.value, grid);
            }
        }

        // just remove grids
        private void removeLine(Dimention dimention, int value)
        {
            int index = 0;
            while(index != grids.Count)
            {
                if (isOnLine(grids[index], dimention, value))
                {
                    GameObject.Destroy(grids[index]);
                    grids.RemoveAt(index);
                }
                else
                    ++index;
            }
        }

        // just add grids
        private void addLine(Point2D start, Vector2D vector, int dist)
        {
            while(dist > 0)
            {
                addGridAt(start);
                start.MoveFor(vector, 1);
                --dist;
            }
        }
       
        private void ShowMap()
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

        private Point2D createPoint(int x, int y)
        {
            Point2D point = this.center.Copy();
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
            return createPoint(x, y);
        }

        private bool isMove = false;
        private void GameObjectMove(Pair objPair)
        {
            if(objPair.obj is Animal)
            {
                Animal animal = (Animal)objPair.obj;
                Vector2D vector = animal.vectorOnScenen;
                Vector2 vect = ConvertTo(vector);
                
                objPair.binded.transform.Translate(vect);

                if (objPair.obj == player)
                    this.isMove = true;
            }
            else
            {
                Debug.Log("error: Maze.MapManager.GameObjectMove() -> gameObject is not Animal");
            }
        }

        private void AnimalChangeColor(GameObject animal, Color color)
        {
            animal.GetComponent<SpriteRenderer>().color = color;
        }
        
        private void updateObject(Pair objPair)
        {
            switch (objPair.obj.GetEvent())
            {
                case Maze.ObjEvent.move:
                    GameObjectMove(objPair);
                    break;
                    
                case Maze.ObjEvent.shape:
                    objPair.binded.GetComponent<SpriteRenderer>().sprite = objPair.obj.Shape();
                    break;

                case ObjEvent.Destroy:
                    GameObject.Destroy(objPair.binded);
                    objPair.binded = null;
                    break;

                case ObjEvent.Grow:
                    float scale = ((Creater)objPair.obj).getLevel() / 10f + 1;
                    objPair.binded.transform.localScale = new Vector2(scale,scale);
                    break;

                case Maze.ObjEvent.None:
                    break;
            }
        }

        private bool isOutOfBuffer(MazeObject obj)
        {
            if (!obj.position.IsOnPlain(this.player.plain))
            {
                return true;
            }
            Point2D position = obj.PositOnScene;
            return (
                position.X.value < center.X.value - extra ||
                position.X.value > center.X.value + extra ||
                position.Y.value < center.Y.value - extra ||
                position.Y.value > center.Y.value + extra);
        }

        private bool haveObj(MazeObject obj)
        {
            foreach(Pair each in objs)
            {
                if (each.obj == obj)
                    return true;
            }
            return false;
        }

        private void addObjInBuffer()
        {
            Iterator iter = new Iterator(center, extra);

            do
            {
                Point2D point = iter.Iter;
                Grid grid = map.GetAt(point);
                if (grid != null && grid.Obj != null)
                {
                    if (!haveObj(grid.Obj))
                        CreateObjAt(point.X.value, point.Y.value, grid.Obj);
                }
            } while (iter.MoveToNext());
        }

        private void removeObjOutBuffer()
        {
            int i = 0;
            while (i < objs.Count)
            {
                Pair each = objs[i];

                if (isOutOfBuffer(each.obj))
                {
                    GameObject.Destroy(each.binded);
                    objs.RemoveAt(i);
                }
                else
                    ++i;
            }
        }
        


        public MapManager(Map2D map, Animal player, GameObject camera, int extra)
        {
            this.player = player;
            this.camera = camera;
            this.map = map;
            this.center = player.PositOnScene.Copy();
            this.extra = extra;
            this.grids = new List<GameObject>();
            this.objs = new List<Pair>();

            gameOver = false;
            camera.transform.position = new Vector3(center.X.value, center.Y.value, -1);
            ShowMap();
        }

        // just change grids
        public void moveForward(Vector2D vector)
        {
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

            removeLine(dimention, value);

            this.center.MoveFor(vector,1);
            this.camera.transform.Translate(ConvertTo(vector));

            Point2D point = this.center.Copy();
            point.MoveFor(vector, extra);
            vector = VectorConvert.Rotate(vector);
            point.MoveFor(vector,extra);
            vector = VectorConvert.Invert(vector);

            addLine(point, vector, width);
        }

        public void changePlain()
        {
            ClearMap();
            this.center = player.PositOnScene.Copy();
            camera.transform.position = new Vector3(center.X.value, center.Y.value, -1);
            ShowMap();
        }

        public bool gameOver
        {
            private set { GlobalAsset.gameOver = value; }
            get { return GlobalAsset.gameOver; }
        }

        public void updateScene()
        {
            if (gameOver) return;

            updateObject(FindMazeObject(player));
            
            if (isMove)
            {
                isMove = false;
                moveForward(player.vectorOnScenen);
            }
            
            removeObjOutBuffer();
            
            addObjInBuffer();
            
            for (int i=0; i<objs.Count;++i)
            {
                updateObject(objs[i]);
                if(objs[i].binded == null)
                {
                    if (objs[i].obj == player)
                        gameOver = true;
                    objs.RemoveAt(i);
                    --i;
                    continue;
                }

                if (objs[i].obj is Animal)
                {
                    Animal animal = (Animal)objs[i].obj;
                    AnimalChangeColor(objs[i].binded, animal.GetColor);
                }
            }
        }

        public Pair FindMazeObject(MazeObject mazeObject)
        {
            foreach(Pair each in objs)
            {
                if (each.obj == mazeObject)
                    return each;
            }
            return null;
        }
    }
}
