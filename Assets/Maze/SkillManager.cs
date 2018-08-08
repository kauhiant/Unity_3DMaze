using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    public class SkillManager
    {
        // 目前在場上出現的技能效果.
        private static List<GameObject> skillObjs = new List<GameObject>();

        // 將場上所有技能效果清除.
        public static void clear()
        {
            while(skillObjs.Count != 0)
            {
                GameObject.Destroy(skillObjs[0]);
                skillObjs.RemoveAt(0);
            }
        }

        // [以施術者為主，需要轉向]
        // 在場上顯示技能效果.(會轉向，會縮放)
        // userPosition 施術者位置.
        // userVector   施術者面向的方位.
        // depthScale   技能效果的深度.
        // widthScale   技能效果的寬度.
        // skill        技能效果的圖片.
        // 回傳         技能效果的GameObject.
        private static GameObject showSkill(Vector2 userPosition, Vector2 userVector, int depthScale, int widthScale, Sprite skill)
        {
            // 如果這個技能玩家不該看到，就不顯示.
            if (userVector.Equals(Vector2.zero)) return null;

            // 創造一個物件.
            float dist = depthScale / 2f + 0.5f;
            GameObject skillObj = new GameObject();

            // 讓他移動到指定位置.
            skillObj.transform.position = userPosition;
            skillObj.transform.Translate(userVector * dist);

            // 放大到適當尺寸.
            skillObj.transform.localScale = new Vector2(depthScale, widthScale);

            // 將這個物件的圖片設定為指定的圖片，並放在技能專屬的圖層.
            skillObj.AddComponent<SpriteRenderer>().sprite = skill;
            skillObj.GetComponent<SpriteRenderer>().sortingLayerName = "action";

            // 轉向.
            skillObj.transform.Rotate(angle(userVector));

            // 將此物件加入[目前場上出現的技能效果]這個List.
            skillObjs.Add(skillObj);
            return skillObj;
        }

        // [以一個點為主，不需要轉向]
        // 在場上顯示技能效果.(在一個點顯示一張圖片)
        // position 技能效果的位置.
        // skill    技能效果的圖片.
        // 回傳     技能效果的GameObject.
        private static GameObject showSkill(Vector2 position, Sprite skill)
        {
            // 在一個位置創造一個物件.
            GameObject skillObj = new GameObject();
            skillObj.transform.position = position;
            
            // 將這個物件的圖片設定為指定的圖片，並放在技能專屬的圖層.
            skillObj.AddComponent<SpriteRenderer>().sprite = skill;
            skillObj.GetComponent<SpriteRenderer>().sortingLayerName = "action";
          
            // 將此物件加入[目前場上出現的技能效果]這個List.
            skillObjs.Add(skillObj);
            return skillObj;
        }


        // [根據技能種類執行不同動作]
        // 在場上顯示技能效果.
        // skill        技能名稱.
        // userPosition 施術者位置.
        // userVector   施術者面向方位.
        private static void showSkill(Skill skill, Point2D userPosition, Vector2D userVector) 
        {
            // 如果還沒指定玩家，就不會畫圖.
            if (!userPosition.IsOnPlain(GlobalAsset.player.Plain))
                return;

            switch (skill)
            {
                case Skill.attack:
                    showSkill(convert(userPosition), convert(userVector), 1, 1, GlobalAsset.attack);
                    break;

                case Skill.straight:
                    showSkill(convert(userPosition), convert(userVector), 3, 1, GlobalAsset.straight);
                    break;

                case Skill.horizon:
                    showSkill(convert(userPosition), convert(userVector), 1, 3, GlobalAsset.horizon);
                    break;

                case Skill.create:
                    showSkill(convert(userPosition), GlobalAsset.create);
                    break;
            }
        }


        // [給 Animal 或 Creater (施術者) 使用]
        public static void showSkill(Skill skill, Point3D position, Vector3D userVector)
        {
            if (GlobalAsset.player == null) return;
            showSkill(skill, positionOnScene(position), vectorOnScene(userVector));
        }

        
        private static Vector3 angle(Vector2 vector)
        {
            if (vector.Equals(Vector2.right))
                return new Vector3(0, 0, 0);
            else if (vector.Equals(Vector2.up))
                return new Vector3(0, 0, 90);
            else if (vector.Equals(Vector2.left))
                return new Vector3(0, 0, 180);
            else if (vector.Equals(Vector2.down))
                return new Vector3(0, 0, -90);
            else
                return Vector3.zero;
        }
        
        private static Vector2 convert(Point2D point)
        {
            return new Vector2(point.X.value, point.Y.value);
        }
        
        private static Vector2 convert(Vector2D vector)
        {
            switch (vector)
            {
                case Vector2D.Right:
                    return Vector2.right;
                case Vector2D.Up:
                    return Vector2.up;
                case Vector2D.Left:
                    return Vector2.left;
                case Vector2D.Down:
                    return Vector2.down;
                default:
                    return Vector2.zero;
            }
        }

        private static Point2D positionOnScene(Point3D position)
        {
            return new Point2D(position, GlobalAsset.player.Plain.Dimention);
        }

        private static Vector2D vectorOnScene(Vector3D vector)
        {
            return GlobalAsset.player.Plain.Vector3To2(vector);
        }

    }
}
