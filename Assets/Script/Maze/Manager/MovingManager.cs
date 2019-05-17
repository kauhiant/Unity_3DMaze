using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    public class MovingManager
    {
        private static List<MovingObj> movingObjs = new List<MovingObj>();

        // 給外部每次 update() 呼叫.
        public static void Update()
        {
            float deltaTime = Time.deltaTime;
            foreach (MovingObj each in movingObjs)
            {
                each.Move(deltaTime);
            }
        }

        // [需要平順移動] 
        // GameObject 移動.
        public static void AddMoving(GameObject gameObject, Vector2 vector)
        {
            movingObjs.Add(new MovingObj(gameObject, vector));
        }

        // [平順移動]
        // 所有移動中物件都移到目標點.
        public static void AllObjsMoveToDest()
        {
            while (movingObjs.Count != 0)
            {
                movingObjs[0].MoveToDest();
                movingObjs.RemoveAt(0);
            }
        }

        public static void RemoveMovingAndMoveTo(GameObject gameObject)
        {
            MovingObj target = null;

            for(int i=0; i<movingObjs.Count; ++i)
            {
                if(movingObjs[i].obj == gameObject)
                {
                    target = movingObjs[i];
                    movingObjs.RemoveAt(i);
                    break;
                }
            }
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

        private float dist = 0;

        public MovingObj(GameObject obj, Vector2 vector)
        {
            this.obj = obj;
            this.vector = vector;
            this.destination = (Vector3)obj.transform.position + (Vector3)vector;
        }

        public void Move(float deltaTime)
        {
            if (obj != null && dist<ClockTime)
            {
                obj.transform.Translate(vector * deltaTime / ClockTime);
                dist += deltaTime;
            }
        }

        public void MoveToDest()
        {
            if (obj != null)
                obj.transform.position = destination;
        }
    }
}
