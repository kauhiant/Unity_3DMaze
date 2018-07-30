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
        public int getLevel() { return level; }

        public Creater(Point3D position, Color color) : base(position)
        {
            this.posit = new Point2D(position, Dimention.Z);
            this.color = color;
            this.level = 0;
            this.energy = 200;
            this.consume = 50;
            this.grow = 300;
            this.rate = 0.01f;

            levelUp();
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

            int createIndex = -1;
            if (UnityEngine.Random.value < rate)
                createIndex = UnityEngine.Random.Range(0, iter.size());

            do
            {
                Point2D point = iter.Iter;
                Grid grid = GlobalAsset.map.GetAt(point.binded);

                if(grid != null)
                    updateForObj(grid.obj);

                if (createIndex-- == 0)
                    CreateAnimal(point.binded.Copy());

            } while (iter.MoveToNext());
            
            if (energy > Math.Pow(2,level) * grow)
                levelUp();
            
            if (this.color.Equals(GlobalAsset.player.color))
                Debug.Log(energy);
        }



        private void levelUp()
        {
            if (level == 10) return;
            ++level;
            Debug.Log("level up " + color);
            RegisterEvent(ObjEvent.Grow);

            Iterator iter = new Iterator(this.posit, this.level + 1);

            do
            {
                Point2D point = iter.Iter;
                Grid grid = GlobalAsset.map.GetAt(point.binded);

                if (grid == null || grid.obj == null)
                    continue;

                if (grid.obj is Stone && !(grid.obj is Creater))
                {
                    grid.obj.RegisterEvent(ObjEvent.Destroy);
                    grid.obj = null;
                }

            } while (iter.MoveToNext());

        }

        private void CreateAnimal(Point3D position)
        {
            if (energy < consume*(level+1)) return;

            Grid grid = GlobalAsset.map.GetAt(position);
            if (grid == null) return;
            if (grid.obj != null) return;

            reduceEnergy(consume);
            grid.obj = new Animal(position, this.color, 10);
            SkillManager.showSkill(Skill.create, new Point2D(position,GlobalAsset.player.plain.dimen), Vector2D.Right);
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
        }
    }
}
