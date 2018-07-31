using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    public class Iterator
    {
        private int extra;

        public Point2D Iter { get; private set; }
        public int Size
        {
            get
            {
                return (int)Math.Pow(extra * 2 + 1, 2);
            }
        }

        public Iterator(Point2D center, int extra)
        {
            this.extra = extra;
            this.Iter = center.Copy();
            this.Iter.MoveFor(Vector2D.Left, extra);
            this.Iter.MoveFor(Vector2D.Down, extra);
        }
        
        private int countX = 0;
        private int countY = 0;
        public bool MoveToNext()
            {
                if (countX == extra * 2)
                {
                    Iter.MoveFor(Vector2D.Left, countX);
                    Iter.MoveFor(Vector2D.Up, 1);
                    countX = 0;
                    ++countY;

                    if(countY == extra * 2+1)
                    {
                        return false;
                    }

                    return true;
                }


                Iter.MoveFor(Vector2D.Right,1);
                ++countX;
                return true;
            }
        
    }    
    
}
