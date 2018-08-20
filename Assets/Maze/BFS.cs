using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    class BFS_Status
    {
        public BFS_Point point;
        public Vector2D vector;
        public BFS_Status lastStatus;

        public BFS_Status(BFS_Point point, Vector2D vector, BFS_Status lastStatus)
        {
            this.point = point;
            this.vector = vector;
            this.lastStatus = lastStatus;
        }
    }

    class BFS_Point
    {
        public int x;
        public int y;
         
        public BFS_Point(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public BFS_Point VectorOf(Vector2D vector)
        {
            switch (vector)
            {
                case Vector2D.Up:
                    return new BFS_Point(x, y + 1);

                case Vector2D.Down:
                    return new BFS_Point(x, y - 1);

                case Vector2D.Left:
                    return new BFS_Point(x - 1, y);

                case Vector2D.Right:
                    return new BFS_Point(x + 1, y);

                default:
                    return null;
            }
        }

        public bool OutOfRange(int x,int y)
        {
            return (this.x < 0 || this.y < 0 || this.x >= x || this.y >= y);
        }

        public bool Equals(BFS_Point point)
        {
            return this.x == point.x
                && this.y == point.y;
        }
    }

    public class BFS_Map
    {
        private Point2D start;
        private Point2D dest;
        private int extra;
        private int width;
        private bool[,] passableMatrix;

        private Map2D sceneMap = new Map2D(MazeObject.World);


        public BFS_Map(Point2D start, Point2D dest, int extra)
        {
            this.start = start;
            this.dest = dest;
            this.extra = extra;
            this.width = extra * 2 + 1;
            this.passableMatrix = new bool[width, width];

            Iterator iterator = new Iterator(start, extra);

            for(int y = 0; y < width; ++y)
            {
                for(int x = 0; x < width; ++x)
                {
                    Grid grid = sceneMap.GetAt(iterator.Iter);

                    if (grid == null)
                        passableMatrix[x, y] = false;
                    else if (grid.IsEmpty())
                        passableMatrix[x, y] = true;
                    else if (grid.Obj is Stone || grid.Obj is Creater)
                        passableMatrix[x, y] = false;
                    else
                        passableMatrix[x, y] = true;

                    iterator.MoveToNext();
                }
            }
        }


        public Stack<Vector2D> FindRoute()
        {
            bool canArrive = false;
            Queue<BFS_Status> routeTree = new Queue<BFS_Status>();
            routeTree.Enqueue(new BFS_Status(Convert(start), Vector2D.Null, null));
            this.SetNotPassAble(Convert(start));

            while(routeTree.Count != 0)
            {
                var status = routeTree.Dequeue();


                // 看這個點的上下左右能不能通過，是不是目的地.
                // 如果能通過就加入queue.
                if (DeStatusForVector(routeTree, status, Vector2D.Up))
                {
                    canArrive = true;
                    break;
                }

                if (DeStatusForVector(routeTree, status, Vector2D.Down))
                {
                    canArrive = true;
                    break;
                }

                if (DeStatusForVector(routeTree, status, Vector2D.Left))
                {
                    canArrive = true;
                    break;
                }

                if (DeStatusForVector(routeTree, status, Vector2D.Right))
                {
                    canArrive = true;
                    break;
                }
            }

            if (!canArrive)
                return null;

            var route = new Stack<Vector2D>();
            var targetStatus = routeTree.Last();

            // 依照lastStatus一路找回原點.
            while (targetStatus.vector != Vector2D.Null)
            {
                route.Push(targetStatus.vector);
                targetStatus = targetStatus.lastStatus;
            }

            return route;
        }

        // 如果這個點剛好是目的地，回傳true.
        private bool DeStatusForVector(Queue<BFS_Status> routeTree, BFS_Status status, Vector2D vector)
        {
            var point = status.point.VectorOf(vector);

            if (point.OutOfRange(width, width))
                return false;

            if (IsPassable(point))
            {
                routeTree.Enqueue(new BFS_Status(point, vector, status));
                SetNotPassAble(point);
            }
            if (IsDest(point))
                return true;

            return false;
        }

        private bool IsPassable(BFS_Point point)
        {
            return passableMatrix[point.x, point.y];
        }

        private void SetNotPassAble(BFS_Point point)
        {
            passableMatrix[point.x, point.y] = false;
        }

        private bool IsDest(BFS_Point point)
        {
            return Convert(dest).Equals(point);
        }
        
        private BFS_Point Convert(Point2D point)
        {
            int x = point.X.value - start.X.value + extra;
            int y = point.Y.value - start.Y.value + extra;
            return new BFS_Point(x, y);
        }
    }

    

}
