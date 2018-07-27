using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    public class SkillManager
    {
        private static List<GameObject> skillObjs = new List<GameObject>();

        public static void clear()
        {
            while(skillObjs.Count != 0)
            {
                GameObject.Destroy(skillObjs[0]);
                skillObjs.RemoveAt(0);
            }
        }

        private static GameObject showSkill(Vector2 userPosition, Vector2 userVector, int depthScale, int widthScale, Sprite skill)
        {
            if (userVector.Equals(Vector2.zero)) return null;

            float dist = depthScale / 2f + 0.5f;
            GameObject skillObj = new GameObject();
            skillObj.transform.position = userPosition;
            skillObj.transform.Translate(userVector * dist);
            skillObj.transform.localScale = new Vector2(widthScale, depthScale);

            skillObj.AddComponent<SpriteRenderer>().sprite = skill;
            skillObj.GetComponent<SpriteRenderer>().sortingLayerName = "action";
            skillObj.transform.Rotate(angle(userVector));

            skillObjs.Add(skillObj);
            return skillObj;
        }

        public static void showSkill(Skill skill, Point2D userPosition, Vector2D userVector) 
        {
            if (!userPosition.isOnPlain(GlobalAsset.player.plain))
                return;

            switch (skill)
            {
                case Skill.attack:
                    showSkill(convert(userPosition), convert(userVector), 1, 1, GlobalAsset.attack);
                    break;
            }
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
            return new Vector2(point.x.value, point.y.value);
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
    }
}
