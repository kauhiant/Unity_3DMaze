using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    class LittleMap
    {
        class Mark
        {
            public Creater creater;
            public int seenLevel;

            public Mark(Creater creater)
            {
                this.creater = creater;
                Update();
            }

            public void Update()
            {
                seenLevel = creater.Range;
            }
        }

        private List<Mark> objsForLittleMap;
        private Map2D map;

        // LittleMap
        private GraphicTest SmallMap { get { return GlobalAsset.smallMap; } }

        private Animal Player { get { return GlobalAsset.player; } }

        public LittleMap(GameObject littleMap, Map2D map)
        {
            this.objsForLittleMap = new List<Mark>();
            this.map = map;
        }



        public void ClearMap()
        {
            SmallMap.Clear();
            objsForLittleMap.Clear();
        }

        public void UpdateForPlayer(List<Creater> creaters)
        {
            SmallMap.Clear();

            int x, y, s;
            foreach (var e in creaters)
            {
                if (!e.PositOnScene.Plain.IsEqual(Player.PositOnScene.Plain))
                    continue;

                var creater = e;
                x = creater.PositOnScene.X.value;
                y = creater.PositOnScene.Y.value;
                s = e.Range * 2;
                SmallMap.DrawGridAt(x, y, creater.GetColor(), s);
            }

            var player = Player;
            x = player.PositOnScene.X.value;
            y = player.PositOnScene.Y.value;
            s = 3;
            SmallMap.DrawGridAt(x, y, player.GetColor(), s, true);

            SmallMap.UpdateGraphic();
        }

        public void UpdateForPlayer()
        {
            UpdateMarksInRange(Player.PositOnScene, GlobalAsset.seenRange);
            CreateMarksInRange(Player.PositOnScene, GlobalAsset.seenRange);
            
            SmallMap.Clear();
            
            int x, y, s;
            foreach(var e in objsForLittleMap)
            {
                var creater = e.creater;
                x = creater.PositOnScene.X.value;
                y = creater.PositOnScene.Y.value;
                s = e.seenLevel * 2;
                SmallMap.DrawGridAt(x, y, creater.GetColor(), s);
            }

            var player = Player;
            x = player.PositOnScene.X.value;
            y = player.PositOnScene.Y.value;
            s = 3;
            SmallMap.DrawGridAt(x, y, player.GetColor(), s, true);

            SmallMap.UpdateGraphic();
        }

        
        private void UpdateMarksInRange(Point2D center, int extra)
        {
            for (int i = 0; i < objsForLittleMap.Count; ++i)
            {
                var each = objsForLittleMap[i];

                if (each.creater.PositOnScene.DistanceTo(center.Binded) > extra)
                    continue;
                else if (each.creater.IsDead)
                    objsForLittleMap.RemoveAt(i--);
                else
                    each.Update();
            }
        }

        private void CreateMarksInRange(Point2D center, int extra)
        {
            Iterator iter = new Iterator(center, extra);

            do
            {
                Point2D point = iter.Iter;
                Grid grid = map.GetAt(point);

                if (grid == null || grid.Obj == null)
                    continue;

                if (grid.Obj is Creater)
                    CreateMarkFor((Creater)grid.Obj);

            } while (iter.MoveToNext());
        }


        // 創造一個 Mark 在 List.
        private void CreateMarkFor(Creater creater)
        {
            // 已存在:不執行.
            foreach(var e in objsForLittleMap)
            {
                if (e.creater == creater)
                    return;
            }

            // 不存在:加入.
            objsForLittleMap.Add(new Mark(creater));
        }

    }
}
