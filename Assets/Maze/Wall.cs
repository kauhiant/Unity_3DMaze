using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    class Wall : MazeObject,Attackable
    {
        private int hp;
        public Wall(Point3D position, int hp) : base(position)
        {
            this.hp = hp;
        }

        public void BeAttack(Animal animal)
        {
            hp -= animal.power;
            if (hp < 0)
                Destroy();
        }

        public override Sprite GetSprite()
        {
            return GlobalAsset.wallSprite;
        }

        
    }
}
