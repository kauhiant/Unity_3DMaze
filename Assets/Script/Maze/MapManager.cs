using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    // 負責 Maze.MazeObject 和 Unity.GameObject 之間的溝通.
    // 如果 Unity 的 camera 可以自動跟隨 PlayerBind ，就不需要這裡的 camera.
    public class MapManager
    {

        // BufferMap
        private GameObject camera;   // main camera.
        private BufferMap bufferMap;

        // LittleMap
        private LittleMap littleMap;

        // Player
        public Animal Player
        { get { return GlobalAsset.player; } }
        public BindObject PlayerBind { get {return bufferMap.PlayerBind; } }
        

        
        public MapManager(Map2D map, GameObject camera, GameObject littleMap, int extra)
        {
            this.camera = camera;
            this.bufferMap = new BufferMap(map, Player.PositOnScene.Copy(), extra);

            this.littleMap = new LittleMap(littleMap, map);

            
            camera.transform.position = new Vector3(bufferMap.center.X.value, bufferMap.center.Y.value, -1);
            bufferMap.ShowMap();
        }
        
        

        // 用來做平順移動.
        // 在MonoBehaviour 的 Update() 呼叫.
        // [用來平順移動]
        // player 不存在，不會執行.
        public void Update(float deltaTime)
        {
            if (Player == null || GameStatus.pause) return;

            MovingManager.Update();
        }

        // 若 player 不存在 不會執行.
        public void Clock()
        {
            if (Player == null || GameStatus.pause) return;

            // 所有移動中物件移到目標點.
            MovingManager.AllObjsMoveToDest();

            // update player
            if (!Player.IsDead)
            {
                var events = PlayerBind.UpdateBinded();
                foreach(var objEvent in events)
                {
                    switch (objEvent)
                    {
                        case ObjEvent.posit:
                            bufferMap.MoveForward(Player.VectorOnScenen);
                            MovingManager.AddMoving(camera, Convert(Player.VectorOnScenen));//bug
                            break;
                        case ObjEvent.plain:
                            RebindPlayer();//bug
                            break;
                    }
                }

            }
                
            

            bufferMap.Clock();

            littleMap.UpdateForPlayer(GlobalAsset.creaters);
        }

        // 當 player 換人時，要呼叫此方法.
        // 不然上一個玩家的視角會殘留.
        public void RebindPlayer()
        {
            bufferMap.ReBindPlayer();
            littleMap.ClearMap();
            camera.transform.position = new Vector3(bufferMap.center.X.value, bufferMap.center.Y.value, -1);
            MovingManager.RemoveMovingAndMoveTo(camera);
        }
        
        
        

        // 將 Maze 的方向轉成 Unity 的方向.
        private static Vector2 Convert(Vector2D vector)
        {
            switch (vector)
            {
                case Maze.Vector2D.Up:
                    return Vector2.up;
                case Maze.Vector2D.Down:
                    return Vector2.down;
                case Maze.Vector2D.Left:
                    return Vector2.left;
                case Maze.Vector2D.Right:
                    return Vector2.right;
                default:
                    return Vector2.zero;
            }
        }
    }
}
