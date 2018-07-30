using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    public class Creater : Stone
    {
        private Point2D posit;
        public Color color;
        private int level;
        private int energy;
        private int consume;
        private int grow;
        private float rate;

        public Color getColor() { return color; }
        public int getLeval() { return level; }

        public Creater(Point3D position, Color color) : base(position)
        {
            this.posit = new Point2D(position, Dimention.Z);
            this.color = color;
            this.level = 1;
            this.energy = 200;
            this.consume = 50;
            this.grow = 300;
            this.rate = 0.1f;
        }

        public override Sprite Shape()
        {
            return GlobalAsset.createrSprite;
        }

        public bool isDead()
        {
            return energy <= 0;
        }

        public void update()
        {
            if (isDead()) return;
            reduceEnergy(1);

            Iterator iter = new Iterator(this.posit, this.level + 1);
            int createIndex = UnityEngine.Random.Range(0, iter.size());

            do
            {
                Point2D point = iter.Iter;
                Grid grid = GlobalAsset.map.GetAt(point.binded);

                if(grid != null)
                    updateForObj(grid.obj);

                if (createIndex-- == 0)
                    CreateAnimal(point.Copy());

            } while (iter.MoveToNext());
            

            if (energy > Math.Pow(level,2) * grow)
                levelUp();
        }



        private void levelUp()
        {
            if (level == 10) return;
            ++level;
            Debug.Log("level up " + color);
            RegisterEvent(ObjEvent.Grow);
        }

        private void CreateAnimal(Point2D position)
        {
            if (energy < consume*(level+1)) return;

            Grid grid = GlobalAsset.map.GetAt(position.binded);
            if (grid == null) return;
            if (grid.obj != null) return;

            reduceEnergy(consume);
            grid.obj = new Animal(position.binded, this.color, 10);
            SkillManager.showSkill(Skill.create, position, Vector2D.Right);
            GlobalAsset.animals.Add((Animal)grid.obj);
        }

        private void updateForObj(MazeObject obj)
        {
            if (obj == null || obj == this) return;

            if(obj is Animal)
            {
                Animal animal = (Animal)obj;
                if (animal.color.Equals(this.color))
                    ++this.energy;
                else
                    reduceEnergy(1);
            }
            else if(obj is Creater)
            {
                Creater creater = (Creater)obj;
                if (creater.color.Equals(this.color))
                    eatCreater(creater);
                else
                    reduceEnergy( creater.level);
            }
        }

        private void eatCreater(Creater creater)
        {
            if (this.level <= creater.level) return;

            this.energy += creater.energy;
            creater.energy = 0;
            creater.destroy();
        }

        private void reduceEnergy(int value)
        {
            this.energy -= value;
            if (energy <= 0)
            {
                energy = 0;
                destroy();
            }
        }

        private void destroy()
        {
            GlobalAsset.map.GetAt(this.position).obj = null;
            this.RegisterEvent(ObjEvent.Destroy);
            
             Debug.Log("destroyed " + this.color);
        }
    }
}
