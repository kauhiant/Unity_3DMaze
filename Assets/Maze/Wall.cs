using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    class Wall : MazeObject,Attackable
    {
        static private Sprite Sprite;
        static public void SetSprite(Sprite sprite)
        {
            Wall.Sprite = sprite;
        }


        private int hp;
        public Wall(Point3D position, int hp) : base(position)
        {
            this.hp = hp;
        }

        public void BeAttack(Animal animal)
        {
            hp -= animal.Power;
            if (hp < 0)
                Destroy();
        }

        public override Sprite GetSprite()
        {
            return Wall.Sprite;
        }

        
    }
}
