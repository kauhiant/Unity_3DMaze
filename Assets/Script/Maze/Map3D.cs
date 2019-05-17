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


        public Map3D(int widthX, int widthY, int layers)
        {
            MazeObject.SetMaze(this);

            map = new Grid[widthX][][];
            for (int i = 0; i < widthX; ++i)
            {
                map[i] = new Grid[widthY][];
                for (int j = 0; j < widthY; ++j)
                {
                    map[i][j] = new Grid[layers];
                    for (int k = 0; k < layers; ++k)
                    {
                        map[i][j][k] = new Grid();
                        map[i][j][k].Sprite = GlobalAsset.gridSprites[k];
                    }
                }
            }

            InitMap();
        }


        // 取得該位置的格子.
        // 沒有格子回傳 null.
        public Grid GetAt(Point3D position)
        {
            if (IsInThisMap(position))
                return map[position.X.value][position.Y.value][position.Z.value];
            else
                return null;
        }

        // 將 a,b 兩位置上的物件交換.
        // 也會修正物件的位置.
        public void Swap(Point3D a, Point3D b)
        {
            if (!IsInThisMap(a) || !IsInThisMap(b))
                return;

            MazeObject temp = GetAt(a).TakeOutObj();
            GetAt(a).InsertObj(GetAt(b).TakeOutObj());
            GetAt(b).InsertObj(temp);

            if (GetAt(a).Obj != null)
                GetAt(a).Obj.position.SetBy(a);

            if (GetAt(b).Obj != null)
                GetAt(b).Obj.position.SetBy(b);
        }

        // 取得該 layer 的隨機一個位置.
        // null : layer 超出範圍.
        public Point3D GetRandomPointOn(int layer)
        {
            if (layer < 0 && layer > this.Layers)
                return null;

            int x = Random.Range(0, WidthX);
            int y = Random.Range(0, WidthY);

            return new Point3D(x, y, layer);
        }


        // 每個 clock 會執行的動作.
        // 將每層各取隨機一點，並對他做動作.
        public void Clock()
        {
            for (int layer = 0; layer < this.Layers; ++layer)
            {
                Point3D point = GetRandomPointOn(layer);
                UpdateGridAt(point);
            }
        }


        // 根據機率在該位置創造食物.
        // 或是創造石頭，或是消除食物.
        public void UpdateGridAt(Point3D position)
        {
            Grid grid = GetAt(position);

            if (grid == null) return;

            if (Random.value < createFoodRate)
            {
                grid.InsertObj(new Food(position, 100));
            }
            else
            {
                if (grid.Obj == null)
                    // CreateStoneAtIfStoneLessThan(position, 2,6);
                    CreateStoneAtIfStoneLessThan(position, 0, 9);
                else if (grid.Obj is Food)
                    grid.Obj.Destroy();
            }
        }

        // 假如 position 是空的，並且它周圍的石頭數量小於 stoneLessThan.
        // 則在 position 創造一顆石頭.
        private void CreateStoneAtIfStoneLessThan(Point3D position, int stoneLessThan, int stoneMoreThan)
        {
            Grid targetGrid = GetAt(position);

            if (targetGrid == null || !targetGrid.IsEmpty())
                return;

            int stoneCount = 0;
            Iterator iter = new Iterator(new Point2D(position, Dimention.Z), 1);

            do
            {
                Point2D point = iter.Iter;
                Grid grid = GetAt(point.Binded);

                if (grid == null)
                {
                    ++stoneCount;
                    continue;
                }

                if (grid.Obj == null)
                    continue;

                if (grid.Obj is Stone)
                    ++stoneCount;

            } while (iter.MoveToNext());

            if (stoneCount < stoneLessThan || stoneCount > stoneMoreThan)
                targetGrid.InsertObj(new Stone(position));
        }

        // 此迷宮裡是否有該位置.
        private bool IsInThisMap(Point3D position)
        {
            return (position.X.value < WidthX &&
                position.Y.value < WidthY &&
                position.Z.value < Layers &&
                position.X.value >= 0 &&
                position.Y.value >= 0 &&
                position.Z.value >= 0);
        }


        // [以下] 迷宮生成.
        private void InitMap()
        {
            for (int i = 0; i < Layers; ++i)
            {
                makePlain(i);
            }
        }

        private void makePlain(int layer)
        {
            MazeGenerator mazeGenerator = new MazeGenerator();

            mazeGenerator.Start_CreatMaze((WidthX + 1) / 2, (WidthY + 1) / 2);

            List<int> coordinate = new List<int>();

            coordinate = mazeGenerator.maze.All_Coordinate();

            int coordinate_range = coordinate.Count;

            for (int i = 0; i < coordinate_range; i += 2)
            {
                Point3D target = new Point3D(coordinate[i], coordinate[i + 1], layer);
                GetAt(target).InsertObj(new Stone(target));
            }
        }

        /*private void makePlain(int layer)
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
        }*/
    }
}

