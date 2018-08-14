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
        class Pair
        {
            public GameObject binded;
            public MazeObject obj;
            public Pair(MazeObject obj, GameObject binded)
            {
                this.obj = obj;
                this.binded = binded;
            }
        }

        class MovingObj
        {
            static public float ClockTime
            {
                get
                {
                    return GlobalAsset.clockTime;
                }
            }

            public GameObject obj;
            public Vector2 vector;
            public Vector3 destination; // 為了配合 camera.

            public MovingObj(GameObject obj, Vector2 vector)
            {
                this.obj = obj;
                this.vector = vector;
                this.destination = (Vector3)obj.transform.position + (Vector3)vector;
            }

            public void Move(float deltaTime)
            {
                if(obj != null)
                    obj.transform.Translate(vector * deltaTime / ClockTime);
            }

            public void MoveToDest()
            {
                if(obj != null)
                    obj.transform.position = destination;
            }
        }

        // GameObjects and MazeObjects
        private List<GameObject> grids;
        private List<Pair> objs;
        private List<Pair> objsForLittleMap;

        // 平順移動(非常需要改進).
        private List<MovingObj> movingObjs;
        
        // RealMap
        private Map2D map;
        private int   mapWidth;

        // BufferMap
        private GameObject camera;   // main camera.
        private Point2D bufferCenter;
        private int     bufferExtra;
        private int     BufferWidth
        { get { return bufferExtra * 2 + 1; } }
        private bool isMove;

        // LittleMap
        private GameObject littleMap; // camera for little map.
        private GameObject playerOnLittleMap;
        private float littleMapWidth = 10;// camera(littleMap)的寬度，不知要怎麼用Unity抓.
        private float littleMapDivBase;
        private Vector2 LittleMapPosition // littleMap 的位置.
        {
            get
            {
                return littleMap.transform.position;
            }
        }
        private float   LittleMapX  // littleMap 的 X 位置.
        {
            get
            {
                return LittleMapPosition.x;
            }
        }
        private float   LittleMapY  // littleMap 的 Y 位置.
        {
            get
            {
                return LittleMapPosition.y;
            }
        }

        // Player
        public Animal Player
        { get { return GlobalAsset.player; } }
        public GameObject PlayerBind
        { get { return FindMazeObjectFrom(objs,Player).binded; } }
        

        
        public MapManager(Map2D map, GameObject camera, GameObject littleMap, int extra)
        {
            this.grids = new List<GameObject>();
            this.objs  = new List<Pair>();
            this.objsForLittleMap = new List<Pair>();
            this.movingObjs = new List<MovingObj>();

            this.map = map;
            this.mapWidth = map.Binded.WidthX;

            this.camera = camera;
            this.bufferCenter = Player.PositOnScene.Copy();
            this.bufferExtra  = extra;
            this.isMove = false;

            this.littleMap = littleMap;
            this.littleMapDivBase = mapWidth / littleMapWidth;

            
            camera.transform.position = new Vector3(bufferCenter.X.value, bufferCenter.Y.value, -1);
            ShowMap();
        }

        
        // 創造 buffer 的 GameObject.
        // Grids and MazeObjects.
        private void ShowMap()
        {
            Iterator iter = new Iterator(bufferCenter, bufferExtra);

            do
            {
                Point2D point = iter.Iter;
                Grid grid = map.GetAt(point);

                if (grid != null)
                {
                    CreateGridAt(point.X.value, point.Y.value, grid);

                    if (grid.Obj != null)
                        CreateObjAt(point.X.value, point.Y.value, grid.Obj);
                }
            } while (iter.MoveToNext());
        }

        // 將場上的 GameObject 清空.
        // 場上此管理器創造出來的.
        private void ClearMap()
        {
            while (grids.Count != 0)
            {
                GameObject.Destroy(grids[0]);
                grids.RemoveAt(0);
            }

            while (objs.Count != 0)
            {
                GameObject.Destroy(objs[0].binded);
                objs.RemoveAt(0);
            }

            while (objsForLittleMap.Count != 0)
            {
                GameObject.Destroy(objsForLittleMap[0].binded);
                objsForLittleMap.RemoveAt(0);
            }

            GameObject.Destroy(playerOnLittleMap);
        }


        // 創造 grid 的綁定物件.
        private void CreateGridAt(int x, int y, Grid grid)
        {
            GameObject temp = new GameObject();
            temp.transform.position = new Vector2(x, y);
            temp.AddComponent<SpriteRenderer>().sprite = Grid.Sprite;
            temp.GetComponent<SpriteRenderer>().sortingLayerName = "grid";

            grids.Add(temp);
        }

        // 創造 MazeObject 的綁定物件.
        private void CreateObjAt(int x, int y, MazeObject obj)
        {
            GameObject temp = new GameObject();
            temp.transform.position = new Vector2(x, y);
            temp.AddComponent<SpriteRenderer>().sprite = obj.GetSprite();
            temp.GetComponent<SpriteRenderer>().color = obj.GetColor();
            temp.GetComponent<SpriteRenderer>().sortingLayerName = "object";

            obj.RegisterEvent(ObjEvent.None);
            objs.Add(new Pair(obj, temp));

            if (obj is Creater)
            {
                temp.GetComponent<SpriteRenderer>().sortingLayerName = "creater";
                temp.transform.localScale = obj.GetScale();

                CreateMarkAtLittleMap(obj);
            }

            else if (obj == Player)
            {
                CreateMarkAtLittleMap(obj);
            }

        }

        
        // 在MonoBehaviour 的 Update() 呼叫.
        // [用來平順移動]
        // player 不存在，不會執行.
        public void Update(float deltaTime)
        {
            if (Player == null) return;

            foreach (MovingObj each in movingObjs)
            {
                each.Move(deltaTime);
            }
        }

        // 若 player 不存在 不會執行.
        public void Clock()
        {
            if (Player == null) return;

            // 所有移動中物件移到目標點.
            ObjsMoveToDest();

            // update player
            if(!Player.IsDead)
                UpdateObject(FindMazeObjectFrom(objs,Player));

            // if player move
            if (isMove)
            {
                // change grids.
                MoveForward(Player.VectorOnScenen);
                isMove = false;
            }

            // change objs.
            RemoveObjOutBuffer();
            AddObjInBuffer();

            // update objs
            for (int i = 0; i < objs.Count; ++i)
            {
                UpdateObject(objs[i]);

                // if the obj is destroyed.
                if (objs[i].binded == null)
                {
                    objs.RemoveAt(i);
                    --i;
                    continue;
                }
                

                /// 目前只有 Animal 會一直改變顏色(alpha)，未來不一定.
                /// 可以把 if 判斷式拿掉，不過可能稍微耗效能.
                if (objs[i].obj is Animal)
                    BindedChangeColor(objs[i]);
            }

            UpdateAllMarkAtLittleMap();
        }

        // 當 player 換人時，要呼叫此方法.
        // 不然上一個玩家的視角會殘留.
        public void ChangePlayer()
        {
            ClearMap();
            this.bufferCenter = Player.PositOnScene.Copy();
            camera.transform.position = new Vector3(bufferCenter.X.value, bufferCenter.Y.value, -1);
            ShowMap();
        }



        // 當 player 改變視角時，要呼叫此方法.
        // 不然 buffer 會看到其他地方.
        private void ChangePlain()
        {
            ClearMap();
            this.bufferCenter = Player.PositOnScene.Copy();
            camera.transform.position = new Vector3(bufferCenter.X.value, bufferCenter.Y.value, -1);
            ShowMap();
        }



        // 將 buffer 往 vector 移動一格.
        // camera 也會更著移動.
        private void MoveForward(Vector2D vector)
        {
            // remove line.
            Dimention dimention = Dimention.Null;
            int value = 0;
            
            switch (vector)
            {
                case Vector2D.Up:
                    dimention = Dimention.Y;
                    value = bufferCenter.Y.value - bufferExtra;
                    break;
                case Vector2D.Down:
                    dimention = Dimention.Y;
                    value = bufferCenter.Y.value + bufferExtra;
                    break;
                case Vector2D.Left:
                    dimention = Dimention.X;
                    value = bufferCenter.X.value + bufferExtra;
                    break;
                case Vector2D.Right:
                    dimention = Dimention.X;
                    value = bufferCenter.X.value - bufferExtra;
                    break;
            }

            RemoveLine(dimention, value);


            // move bufferCenter and camera.
            this.bufferCenter.MoveFor(vector, 1);
            GameObjectMove(camera, Convert(vector));


            // add line.
            Point2D point = this.bufferCenter.Copy();
            point.MoveFor(vector, bufferExtra);
            vector = VectorConvert.Rotate(vector);
            point.MoveFor(vector, bufferExtra);
            vector = VectorConvert.Invert(vector);

            AddLine(point, vector, BufferWidth);
        }
        

        // obj is on the line(dimention:value) ?
        private bool IsOnLine(GameObject obj, Dimention dimention, int value)
        {
            float dimen = 0;
            switch (dimention)
            {
                case Dimention.X:
                    dimen = obj.transform.position.x;
                    break;
                case Dimention.Y:
                    dimen = obj.transform.position.y;
                    break;
                case Dimention.Z:
                    dimen = obj.transform.position.z;
                    break;
            }
            
            return Math.Round(dimen) == value;
        }

        // just add grid
        private void AddGridAt(Point2D point)
        {
            Grid grid = map.GetAt(point);
            if (grid != null)
            {
                CreateGridAt(point.X.value, point.Y.value, grid);
            }
        }

        // just remove grids
        private void RemoveLine(Dimention dimention, int value)
        {
            int index = 0;
            while(index != grids.Count)
            {
                if (IsOnLine(grids[index], dimention, value))
                {
                    GameObject.Destroy(grids[index]);
                    grids.RemoveAt(index);
                }
                else
                    ++index;
            }
        }

        // just add grids
        private void AddLine(Point2D start, Vector2D vector, int dist)
        {
            while(dist > 0)
            {
                AddGridAt(start);
                start.MoveFor(vector, 1);
                --dist;
            }
        }



        // Clock : 把 buffer 內未被加入的 MazeObject 加入場上.
        // 因為 MazeObject 會移動、會死亡、會生成，不向 Grid 是固定的.
        // 所以每段時間都要更新一次.
        private void AddObjInBuffer()
        {
            Iterator iter = new Iterator(bufferCenter, bufferExtra);

            do
            {
                Point2D point = iter.Iter;
                Grid grid = map.GetAt(point);
                if (grid != null && grid.Obj != null)
                {
                    if (!BufferHaveObj(grid.Obj))
                        CreateObjAt(point.X.value, point.Y.value, grid.Obj);
                }
            } while (iter.MoveToNext());
        }

        // Clock : 把超出 buffer 的 MazeObject 從場上移除.
        // 因為 MazeObject 會移動、會死亡、會生成，不向 Grid 是固定的.
        // 所以每段時間都要更新一次.
        private void RemoveObjOutBuffer()
        {
            int i = 0;
            while (i < objs.Count)
            {
                Pair each = objs[i];

                if (IsOutOfBuffer(each.obj))
                {
                    GameObject.Destroy(each.binded);
                    objs.RemoveAt(i);
                }
                else
                    ++i;
            }
        }



        // 將 Maze 的方向轉成 Unity 的方向.
        private Vector2 Convert(Vector2D vector)
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

        // 將 Maze 的座標轉成 Unity 的座標.
        private Vector2 Convert(Point2D point)
        {
            return new Vector2(point.X.value, point.Y.value);
        }

        // 將 Unity 的座標轉成 Maze 的座標.
        private Point2D Convert(Vector2 vector)
        {
            int x = (int)Mathf.Round(vector.x);
            int y = (int)Mathf.Round(vector.y);

            Point2D point = this.bufferCenter.Copy();
            point.X.value = x;
            point.Y.value = y;
            return point;
        }



        // 依據 MazeObject 的註冊事件，更新綁定物件.
        private void UpdateObject(Pair objPair)
        {
            switch (objPair.obj.GetEvent())
            {
                case ObjEvent.move:
                    BindedMove(objPair);
                    break;
                    
                case ObjEvent.shape:
                    objPair.binded.GetComponent<SpriteRenderer>().sprite = objPair.obj.GetSprite();
                    break;

                case ObjEvent.Destroy:
                    GameObject.Destroy(objPair.binded);
                    objPair.binded = null;
                    break;

                case ObjEvent.Grow:
                    objPair.binded.transform.localScale = objPair.obj.GetScale();
                    break;

                case ObjEvent.plain:
                    ChangePlain();
                    break;

                case ObjEvent.None:
                    break;
            }
        }

        // 綁定物件依據 Animal 的方向移動.
        private void BindedMove(Pair objPair)
        {
            if(objPair.obj is Animal)
            {
                Animal animal = (Animal)objPair.obj;
                Vector2 vector = Convert(animal.VectorOnScenen);
                
                GameObjectMove(objPair.binded, vector);

                if (objPair.obj == Player)
                {
                    this.isMove = true;
                    PlayerOnLittleMapMove(vector);
                }
            }
        }

        // 綁定物件依據 MazeObject 的顏色改變.
        private void BindedChangeColor(Pair objPair)
        {
            objPair.binded.GetComponent<SpriteRenderer>().color = objPair.obj.GetColor();
        }


        // [需要平順移動] 
        // GameObject 移動.
        private void GameObjectMove(GameObject gameObject, Vector2 vector)
        {
            //gameObject.transform.Translate(vector);
            movingObjs.Add(new MovingObj(gameObject, vector));
        }

        // [平順移動]
        // 所有移動中物件都移到目標點.
        private void ObjsMoveToDest()
        {
            while(movingObjs.Count != 0)
            {
                movingObjs[0].MoveToDest();
                movingObjs.RemoveAt(0);
            }
        }
        

        // 從 objs 或 objsForLittleMap 中找出綁定物件.
        // null : 找不到.
        private Pair FindMazeObjectFrom(List<Pair> pairs,MazeObject mazeObject)
        {
            foreach(Pair each in pairs)
            {
                if (each.obj == mazeObject)
                    return each;
            }
            return null;
        }

        // obj 的位置是否在 buffer 外?
        private bool IsOutOfBuffer(MazeObject obj)
        {
            if (!obj.position.IsOnPlain(this.Player.Plain))
                return true;

            Point2D position = obj.PositOnScene;
            return (
                position.X.value < bufferCenter.X.value - bufferExtra ||
                position.X.value > bufferCenter.X.value + bufferExtra ||
                position.Y.value < bufferCenter.Y.value - bufferExtra ||
                position.Y.value > bufferCenter.Y.value + bufferExtra);
        }

        // obj 是否已存在場上的 buffer ?
        private bool BufferHaveObj(MazeObject obj)
        {
            foreach(Pair each in objs)
            {
                if (each.obj == obj)
                    return true;
            }
            return false;
        }






        
        // 小地圖上的玩家移動.
        private void PlayerOnLittleMapMove(Vector2 vector)
        {
            playerOnLittleMap.transform.Translate(vector* littleMapWidth/ mapWidth);
        }

        // 創造一個 Creater 或 Player 在小地圖.
        private void CreateMarkAtLittleMap(MazeObject obj)
        {
            if (FindMazeObjectFrom(objsForLittleMap, obj) != null) return;
            if (obj.PositOnScene.Plain.Dimention != Dimention.Z && obj != Player) return;


            float x = obj.PositOnScene.X.value / littleMapDivBase + LittleMapX - littleMapWidth / 2;
            float y = obj.PositOnScene.Y.value / littleMapDivBase + LittleMapY - littleMapWidth / 2;
            Color color;
            float scale = 1;

            if (obj == Player)
            {
                Animal player = (Animal)obj;
                color = player.Color;
            }
            else
            {
                Creater creater = (Creater)obj;
                scale = creater.Level / 5f + 1;
                color = creater.GetColor();
            }
            

            GameObject mark = new GameObject();
            mark.transform.position = new Vector2(x, y);
            mark.transform.localScale = new Vector2(scale, scale);

            if (obj == Player)
            {
                playerOnLittleMap = mark;
                mark.AddComponent<SpriteRenderer>().sprite = GlobalAsset.playerMark;
                mark.GetComponent<SpriteRenderer>().sortingLayerName = "object";
                mark.GetComponent<SpriteRenderer>().color = color;
            }
            else
            {
                objsForLittleMap.Add(new Pair(obj, mark));
                Color alphaColor = new Color(color.r, color.g, color.b, 0.7f);
                mark.AddComponent<SpriteRenderer>().sprite = GlobalAsset.createrMark;
                mark.GetComponent<SpriteRenderer>().sortingLayerName = "creater";
                mark.GetComponent<SpriteRenderer>().color = alphaColor;
            }
        }

        // 小地圖更新，只會在離玩家5步的距離更新.
        // 依據 creater.Level 更改 mark.scale.
        // 若 creater 死了，會移除.
        private void UpdateAllMarkAtLittleMap()
        {
            for(int i=0; i<objsForLittleMap.Count; ++i)
            {
                Pair each = objsForLittleMap[i];
                if (each.obj.PositOnScene.DistanceTo(Player.position) > 5) continue;

                if(each.obj is Creater)
                {
                    Creater creater = (Creater)each.obj;

                    if (creater.IsDead)
                    {
                        objsForLittleMap.Remove(each);
                        GameObject.Destroy(each.binded);
                        --i;
                    }
                    else
                    {
                        float scale = creater.Level / 5f + 1;
                        each.binded.transform.localScale = new Vector2(scale, scale);
                    }
                }

                
            }
        }


    }
}
