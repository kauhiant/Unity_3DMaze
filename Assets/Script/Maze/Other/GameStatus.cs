using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    class GameStatus
    {
        static public bool ready; // 開頭故事講完了嗎?
        static public bool pause; // 是否暫停.
        static public bool win;
        static public bool lose;
        static public bool moved;
        static public bool ate;

        static public void Reset()
        {
            ready = false;
            pause = true;
            win = false;
            lose = false;
            moved = false;
            ate = false;
        }

        static public void Clock()
        {
            if(GlobalAsset.player != null)
            {
                if (initPosition != null)
                {
                    if (!initPosition.Equals(GlobalAsset.player.PositOnScene))
                        moved = true;
                }
                else
                {
                    initPosition = GlobalAsset.player.PositOnScene.Copy();
                }

            }
        }

        static private Point2D initPosition;
    }
}
