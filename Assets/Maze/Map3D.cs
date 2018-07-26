using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    public class Map3D
    {
        private Grid[][][] map;

        public int widthX { get { return map.Length; } }
        public int widthY { get { return map[0].Length; } }
        public int layers { get { return map[0][0].Length; } }

        public Map3D (int widthX, int widthY, int layers)
        {

            map = new Grid[widthX][][];
            for(int i=0; i<widthX; ++i)
            {
                map[i] = new Grid[widthY][];
                for(int j=0; j<widthY; ++j)
                {
                    map[i][j] = new Grid[layers];
                    for(int k=0; k<layers; ++k)
                    {
                        map[i][j][k] = new Grid();
                        map[i][j][k].shape = GlobalAsset.gridSprite;
                    }
                }
            }

            InitMap();
        }
        

        public bool IsInThisMap(Point3D position)
        {
            return (position.x.value < widthX &&
                position.y.value < widthY &&
                position.z.value < layers &&
                position.x.value >= 0 &&
                position.y.value >= 0 &&
                position.z.value >= 0);
        }

        public Grid GetAt(Point3D position)
        {
            if (IsInThisMap(position))
                return map[position.x.value][position.y.value][position.z.value];

            else
                return null;
        }

        // [danger] maybe create a ghost
        public bool HardInsertAt(Point3D position, MazeObject obj)
        {
            if (!IsInThisMap(position))
                return false;

            if (GetAt(position) == null)
                return false;

            GetAt(position).obj = obj;
            return true;
        }

        public bool InsertAt(Point3D position, MazeObject obj)
        {
            if (!IsInThisMap(position))
                return false;                

            if (GetAt(position) == null)
                return false;

            if (GetAt(position).obj != null)
                return false;
            
            GetAt(position).obj = obj;
            return true;
        }

        public void RemoveAt(Point3D position)
        {
            if (IsInThisMap(position))
                GetAt(position).obj = null;
        }

        public void Swap(Point3D a, Point3D b)
        {
            MazeObject temp = GetAt(a).obj;
            GetAt(a).obj = GetAt(b).obj;
            GetAt(b).obj = temp;
            
            if (GetAt(a).obj != null)
                GetAt(a).obj.position.SetBy(a);

            if (GetAt(b).obj != null)
                GetAt(b).obj.position.SetBy(b);
        }

        public void SetBackgroundAt(Point3D position, UnityEngine.Sprite shape)
        {
            if (IsInThisMap(position))
                GetAt(position).shape = shape;
        }

        public bool RandomInsertAt(MazeObject obj, int layer)
        {
            int x = Random.Range(0, widthX);
            int y = Random.Range(0, widthY);

            if(layer >= 0 && layer < this.layers)
            {
                obj.position.Set(x, y, layer);
                return this.InsertAt(obj.position, obj);
            }

            return false;
        }
        

        private void InitMap()
        {
            for(int i=0; i<layers; ++i)
            {
                makePlain(i);
            }
        }


        private void makePlain(int layer)
        {
            for (int j = 0; j <= widthY; j += 2)
            {
                for (int i = 0; i <= widthX; i += 2)
                {
                    int x = Random.Range(0, 100);

                    if (x < 85)
                        createOneStone(i, j, layer);
                    else if (x >= 85 && x <= 95)
                        createTwoStone(i, j, layer);
                    else
                        createThreeStone(i, j, layer);
                }
            }
        }

        private void createOneStone(int x, int y, int layer)
        {
            Point3D target = null;
            switch (Random.Range(0, 3))
            {
                case 0:
                    target = new Point3D(x, y, layer);
                    break;
                case 1:
                    target = new Point3D(x + 1, y, layer);
                    break;
                case 2:
                    target = new Point3D(x, y + 1, layer);

                    break;
                case 3:
                    target = new Point3D(x + 1, y + 1, layer);
                    break;
            }

            if (target != null && GetAt(target) != null)
                GetAt(target).obj = new Stone(target);
        }

        private void createTwoStone(int x, int y, int layer)
        {
            Point3D target1 = null;
            Point3D target2 = null;
            switch (Random.Range(4, 9))
            {
                case 4:
                    target1 = new Point3D(x, y, layer);
                    target2 = new Point3D(x, y + 1, layer);
                    break;
                case 5:
                    target1 = new Point3D(x, y, layer);
                    target2 = new Point3D(x + 1, y, layer);
                    break;
                case 6:
                    target1 = new Point3D(x + 1, y, 0);
                    target2 = new Point3D(x + 1, y - 1, 0);
                    break;
                case 7:
                    target1 = new Point3D(x, y - 1, 0);
                    target2 = new Point3D(x + 1, y - 1, 0);
                    break;
                case 8:
                    target1 = new Point3D(x, y, 0);
                    target2 = new Point3D(x + 1, y - 1, 0);
                    break;
                case 9:
                    target1 = new Point3D(x, y - 1, 0);
                    target2 = new Point3D(x + 1, y, 0);
                    break;
            }

            if (target1 != null && GetAt(target1) != null)
                GetAt(target1).obj = new Stone(target1);
            
            if (target2 != null && GetAt(target2) != null)
                GetAt(target2).obj = new Stone(target2);
        }

        private void createThreeStone(int x, int y, int layer)
        {
            Point3D target1 = null;
            Point3D target2 = null;
            Point3D target3 = null;

            switch (Random.Range(10, 13))
            {
                case 10:
                    target1 = new Point3D(x, y, 0);
                    target2 = new Point3D(x, y - 1, 0);
                    target3 = new Point3D(x + 1, y, 0);
                    break;
                case 11:
                    target1 = new Point3D(x, y, 0);
                    target2 = new Point3D(x + 1, y, 0);
                    target3 = new Point3D(x + 1, y - 1, 0);
                    break;
                case 12:
                    target1 = new Point3D(x, y, 0);
                    target2 = new Point3D(x, y - 1, 0);
                    target3 = new Point3D(x + 1, y - 1, 0);
                    break;
                case 13:
                    target1 = new Point3D(x, y - 1, 0);
                    target2 = new Point3D(x + 1, y, 0);
                    target3 = new Point3D(x + 1, y - 1, 0);
                    break;
            }

            if (target1 != null && GetAt(target1) != null)
                GetAt(target1).obj = new Stone(target1);

            if (target2 != null && GetAt(target2) != null)
                GetAt(target2).obj = new Stone(target2);

            if (target3 != null && GetAt(target3) != null)
                GetAt(target3).obj = new Stone(target3);
        }
    }
}

