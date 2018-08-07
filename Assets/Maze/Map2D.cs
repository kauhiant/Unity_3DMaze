using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    // 用來將 3D 轉 2D 的中間層.
    public class Map2D
    {
        // 要轉換的3維地圖.
        public Map3D Binded { get; private set; }

        public Map2D(Map3D binded)
        {
            this.Binded = binded;
        }

        // 取得該位置的格子.
        // 若沒有回傳 null.
        public Grid GetAt(Point2D position)
        {
            return Binded.GetAt(position.Binded);
        }
    }
}
