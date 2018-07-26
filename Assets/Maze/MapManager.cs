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
                CreateGridAt(point.x.value, point.y.value, grid);
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
                    CreateGridAt(point.x.value, point.y.value, grid);
                    if (grid.obj != null)
                        CreateObjAt(point.x.value, point.y.value, grid.obj);
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
            point.x.value = x;
            point.y.value = y;
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
            return new Vector2(point.x.value, point.y.value);
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

                case Maze.ObjEvent.None:
                    break;
            }
        }

        private bool isOutOfBuffer(MazeObject obj)
        {
            if (!obj.position.isOnPlain(this.player.plain))
            {
                return true;
            }
            Point2D position = obj.positOnScene;
            return (
                position.x.value < center.x.value - extra ||
                position.x.value > center.x.value + extra ||
                position.y.value < center.y.value - extra ||
                position.y.value > center.y.value + extra);
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
                if (grid != null && grid.obj != null)
                {
                    if (!haveObj(grid.obj))
                        CreateObjAt(point.x.value, point.y.value, grid.obj);
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
            this.center = player.positOnScene.Copy();
            this.extra = extra;
            this.grids = new List<GameObject>();
            this.objs = new List<Pair>();

            camera.transform.position = new Vector3(center.x.value, center.y.value, -1);
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
                    value = center.y.value - extra;
                    break;
                case Vector2D.Down:
                    dimention = Dimention.Y;
                    value = center.y.value + extra;
                    break;
                case Vector2D.Left:
                    dimention = Dimention.X;
                    value = center.x.value + extra;
                    break;
                case Vector2D.Right:
                    dimention = Dimention.X;
                    value = center.x.value - extra;
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
            this.center = player.positOnScene.Copy();
            camera.transform.position = new Vector3(center.x.value, center.y.value, -1);
            ShowMap();
        }

        private int count = 0;
        public void updateScene()
        {
            updateObject(FindMazeObject(player));
            
            if (isMove)
            {
                isMove = false;
                moveForward(player.vectorOnScenen);
            }
            
            removeObjOutBuffer();
            
            addObjInBuffer();
            
            for (int i = 0; i < objs.Count; ++i)
            {
                updateObject(objs[i]);
            }

            ++count;
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
