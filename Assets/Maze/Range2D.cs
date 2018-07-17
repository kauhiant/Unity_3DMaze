using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    public class Iterator {
        public Point2D Iter { get { return iter.Copy(); } }

            Point2D iter;
            int extra;

            int countX = 0;
            int countY = 0;

            public Iterator (Point2D center, int extra)
            {
                this.extra = extra;
                this.iter = center.Copy();
                this.iter.MoveFor(Vector2D.Left, extra);
                this.iter.MoveFor(Vector2D.Down, extra);
            }

            public bool MoveToNext()
            {
                if (countX == extra * 2)
                {
                    iter.MoveFor(Vector2D.Left, countX);
                    iter.MoveFor(Vector2D.Up, 1);
                    countX = 0;
                    ++countY;

                    if(countY == extra * 2+1)
                    {
                        return false;
                    }

                    return true;
                }


                iter.MoveFor(Vector2D.Right,1);
                ++countX;
                return true;
            }
        }    
    
}
