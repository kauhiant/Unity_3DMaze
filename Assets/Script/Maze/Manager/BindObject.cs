using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    public class BindObject
    {
        public MazeObject obj;
        public GameObject binded;

        private Transform transform;
        private SpriteRenderer spriteRenderer;
        private BeAttack beAttack;
        

        public BindObject(MazeObject mazeObj)
        {
            this.obj = mazeObj;
            binded = new GameObject("MazeObject");
            transform = binded.transform;

            int x = obj.PositOnScene.X.value;
            int y = obj.PositOnScene.Y.value;
            transform.position = new Vector2(x, y);

            spriteRenderer = binded.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = obj.GetSprite();
            spriteRenderer.color = obj.GetColor();
            spriteRenderer.sortingLayerName = "object";
            
            if (obj is Creater)
            {   // 因為當 Creater 變大時，會有重疊問題，所以要分圖層。
                spriteRenderer.sortingLayerName = "creater";
                binded.transform.localScale = obj.GetScale();
            }

            obj.InitEvents();

            // 為了做滑鼠移入觸發.
            binded.AddComponent<CircleCollider2D>().isTrigger = true ;
            binded.AddComponent<ShowMessage>().refer = this.obj;

            // 明滅閃爍.
            this.beAttack = binded.AddComponent<BeAttack>();
        }

        public void Destroy()
        {
            GameObject.Destroy(binded);
        }

        // 依據 MazeObject 的註冊事件，更新綁定物件.
        public List<ObjEvent> UpdateBinded()
        {
            var objEvents = obj.GetEvents();
            var newColor = obj.GetColor();
            List<ObjEvent> retData = new List<ObjEvent>();

            // 避免重複呼叫重複執行.
            if (objEvents.Count != 0)
            {
                // alpha 會一直變.
                spriteRenderer.color = newColor;
            }
            

            while (objEvents.Count != 0)
            {
                // 把 objEvents 的第一個事件取出來.
                var objEvent = objEvents.ElementAt(0);
                retData.Add(objEvent);
                objEvents.RemoveAt(0);

                // 依據事件決定如何處理對應綁定物件.
                switch (objEvent)
                {
                    case ObjEvent.posit:
                        BindedMove();
                        break;

                    case ObjEvent.scale:
                        transform.localScale = obj.GetScale();
                        break;

                    case ObjEvent.sprite:
                        spriteRenderer.sprite = obj.GetSprite();
                        break;

                    case ObjEvent.damage:
                        beAttack.Flash(GlobalAsset.clockTime, 0.7f, newColor);
                        break;

                    case ObjEvent.Destroy:
                        this.Destroy();
                        binded = null;
                        break;
                }
            }
            return retData;
        }

        // 綁定物件依據 Animal 的方向移動.
        private void BindedMove()
        {
            Animal animal = (Animal)obj;
            MovingManager.AddMoving(binded,Convert(animal.VectorOnScenen));
        }


        // 將 Maze 的方向轉成 Unity 的方向.
        private static Vector2 Convert(Vector2D vector)
        {
            return VectorConvert.Convert(vector);
        }

        // 將 Maze 的座標轉成 Unity 的座標.
        private static Vector2 Convert(Point2D point)
        {
            return VectorConvert.Convert(point);
        }
        
    }
}
