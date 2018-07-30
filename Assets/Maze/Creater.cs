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
        private EnergyBar energy;
        private int consume;
        private float rate;

        public Color getColor() { return color; }
        public int getLevel() { return level; }

        public Creater(Point3D position, Color color) : base(position)
        {
            this.posit = new Point2D(position, Dimention.Z);
            this.color = color;
            this.level = 0;
            this.energy = new EnergyBar(200);
            this.consume = 50;
            this.rate = 0.02f;

            levelUp();
        }

        public override Sprite Shape()
        {
            return GlobalAsset.createrSprite;
        }

        public bool isDead()
        {
            return energy.isZero();
        }

        public Animal update()
        {
            if (isDead()) return null;
            energy.add(-1);

            Iterator iter = new Iterator(this.posit, this.level+1);

            int createIndex = -1;
            Animal animal = null;
            if (UnityEngine.Random.value < rate)
                createIndex = UnityEngine.Random.Range(0, iter.size());

            do
            {
                Point2D point = iter.Iter;
                Grid grid = GlobalAsset.map.GetAt(point.binded);

                if(grid != null)
                    updateForObj(grid.obj);

                if (createIndex-- == 0)
                    animal = CreateAnimal(point.binded.Copy());

            } while (iter.MoveToNext());
            
            if (energy.isFull())
                levelUp();

            if (isDead())
                destroy();

            return animal;
        }



        private void levelUp()
        {
            if (level == 10) return;
            ++level;
            energy.maxExpand(energy.Value);
            Debug.Log("level up " + color.ToString());
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
                    Debug.Log("destroy stone");
                }


            } while (iter.MoveToNext());

        }

        private Animal CreateAnimal(Point3D position)
        {
            if (energy.Value < consume*(level+1)) return null;

            Grid grid = GlobalAsset.map.GetAt(position);
            if (grid == null) return null;
            if (grid.obj != null) return null;

            energy.add(-consume);
            grid.obj = new Animal(position, this.color, 10);
            SkillManager.showSkill(Skill.create, new Point2D(position,GlobalAsset.player.plain.dimen), Vector2D.Right);
            GlobalAsset.animals.Add((Animal)grid.obj);

            return (Animal)grid.obj;
        }

        private void updateForObj(MazeObject obj)
        {
            if (obj == null || obj == this) return;

            if(obj is Animal)
            {
                Animal animal = (Animal)obj;
                if (animal.color.Equals(this.color))
                    energy.add(1);
                else
                    energy.add(-1);
            }
            else if(obj is Creater)
            {
                Creater creater = (Creater)obj;
                if (creater.color.Equals(this.color))
                    eatCreater(creater);
                else
                    energy.add(-creater.level);
            }
        }

        private void eatCreater(Creater creater)
        {
            if (this.level <= creater.level) return;

            energy.add(creater.energy.Value);
            creater.energy.set(0);
            creater.destroy();
        }

        private void destroy()
        {
            GlobalAsset.map.GetAt(this.position).obj = null;
            this.RegisterEvent(ObjEvent.Destroy);
            if (color.Equals(GlobalAsset.player.color))
                Debug.Log("create destroy");
        }
    }
}
