using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
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

        private void addGridAt(Point2D point)
        {
            Grid grid = map.GetAt(point);
            if (grid != null)
            {
                CreateGridAt(point.x.value, point.y.value, grid);
                if (grid.obj != null)
                    CreateObjAt(point.x.value, point.y.value, grid.obj);
            }
        }

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

            index = 0;
            while(index != objs.Count)
            {
                if (isOnLine(objs[index].binded, dimention, value))
                {
                    GameObject.Destroy(objs[index].binded);
                    objs.RemoveAt(index);
                }
                else
                    ++index;

            }
        }

        private void addLine(Point2D start, Vector2D vector, int dist)
        {
            while(dist > 0)
            {
                addGridAt(start);
                start.MoveFor(vector, 1);
                --dist;
            }
        }

        private void updateObj(GameObject obj, string command)
        {
            command = null;
        }

        private void ShowMap()
        {
            Maze.Iterator iter = new Maze.Iterator(center, 8);

            do
            {
                Maze.Point2D point = iter.Iter;

                Maze.Grid grid = map.GetAt(point);
                if (grid != null)
                {
                    CreateGridAt(point.x.value, point.y.value, grid);
                    if (grid.obj != null)
                        CreateObjAt(point.x.value, point.y.value, grid.obj);
                }
            } while (iter.MoveToNext());
        }


        public MapManager(Map2D map, Point2D center, int extra)
        {
            this.map = map;
            this.center = center;
            this.extra = extra;
            this.grids = new List<GameObject>();
            this.objs = new List<Pair>();
            ShowMap();
            
        }

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

            Point2D point = this.center.Copy();
            point.MoveFor(vector, extra);
            vector = VectorConvert.Rotate(vector);
            point.MoveFor(vector,extra);
            vector = VectorConvert.Invert(vector);
            addLine(point, vector, width);
        }

        public void updateScene()
        {

        }
        
        public GameObject FindMazeObject(MazeObject mazeObject)
        {
            foreach(Pair each in objs)
            {
                if (each.obj == mazeObject)
                    return each.binded;
            }
            return null;
        }
    }
}
