using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Maze
{
    public class Map3D
    {
        private Grid[][][] map;
        private float createFoodRate = 0.1f;

        public int WidthX { get { return map.Length; } }
        public int WidthY { get { return map[0].Length; } }
        public int Layers { get { return map[0][0].Length; } }


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
        


        public Grid GetAt(Point3D position)
        {
            if (IsInThisMap(position))
                return map[position.X.value][position.Y.value][position.Z.value];

            else
                return null;
        }

        /// <summary>
        /// return false : the grid of position is not empty, cannot insert
        /// </summary>
        public bool InsertAt(Point3D position, MazeObject obj)
        {
            if (GetAt(position) == null)
                return false;

            return GetAt(position).InsertObj(obj);
        }

        public void RemoveAt(Point3D position)
        {
            if (IsInThisMap(position))
                GetAt(position).RemoveObj();
        }

        // if ghost want to move
        // obj of a and b are null
        // so ghost cannot move
        public void Swap(Point3D a, Point3D b)
        {
            MazeObject temp = GetAt(a).TakeOutObj();
            GetAt(a).InsertObj(GetAt(b).TakeOutObj());
            GetAt(b).InsertObj(temp);
            
            if (GetAt(a).Obj != null)
                GetAt(a).Obj.position.SetBy(a);

            if (GetAt(b).Obj != null)
                GetAt(b).Obj.position.SetBy(b);
        }

        public Point3D GetRandomPointOn(int layer)
        {
            if (layer < 0 && layer > this.Layers)
                return null;

            int x = Random.Range(0, WidthX);
            int y = Random.Range(0, WidthY);

            return new Point3D(x,y,layer);
        }



        public void Clock()
        {
            for(int layer=0; layer<this.Layers; ++layer)
            {
                Point3D point = GetRandomPointOn(layer);
                CreateFoodAt(point);
            }
        }

        public void CreateFoodAt(Point3D position)
        {
            if (UnityEngine.Random.value > createFoodRate)
                return;

            InsertAt(position, new Food(position, 100));
        }
        


        private bool IsInThisMap(Point3D position)
        {
            return (position.X.value < WidthX &&
                position.Y.value < WidthY &&
                position.Z.value < Layers &&
                position.X.value >= 0 &&
                position.Y.value >= 0 &&
                position.Z.value >= 0);
        }

        private void InitMap()
        {
            for(int i=0; i<Layers; ++i)
            {
                makePlain(i);
            }
        }

        private void makePlain(int layer)
        {
            for (int j = 0; j <= WidthY; j += 2)
            {
                for (int i = 0; i <= WidthX; i += 2)
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
                GetAt(target).InsertObj(new Stone(target));
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
                GetAt(target1).InsertObj(new Stone(target1));
            
            if (target2 != null && GetAt(target2) != null)
                GetAt(target2).InsertObj(new Stone(target2));
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
                GetAt(target1).InsertObj(new Stone(target1));

            if (target2 != null && GetAt(target2) != null)
                GetAt(target2).InsertObj(new Stone(target2));

            if (target3 != null && GetAt(target3) != null)
                GetAt(target3).InsertObj(new Stone(target3));
        }
    }
}

