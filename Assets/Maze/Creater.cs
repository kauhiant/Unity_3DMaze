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
        private Color color;
        private int level;
        private EnergyBar energy;
        private int consume;
        private float rate;

        public override Color GetColor() { return color; }
        public override Vector2 GetScale()
        {
            float scale = GetLevel() / 10f + 1;
            return new Vector2(scale, scale);
        }
        public int GetLevel() { return level; }

        public Creater(Point3D position, Color color) : base(position)
        {
            this.posit = new Point2D(position, Dimention.Z);
            this.color = color;
            this.level = 0;
            this.energy = new EnergyBar(200);
            this.consume = 50;
            this.rate = 0.05f;

            levelUp();
        }

        public override Sprite GetSprite()
        {
            return GlobalAsset.createrSprite;
        }

        public bool isDead()
        {
            return energy.IsZero;
        }

        public Animal update()
        {
            if (isDead()) return null;
            energy.Add(-1);

            Iterator iter = new Iterator(this.posit, this.level+1);

            int createIndex = -1;
            Animal animal = null;
            if (UnityEngine.Random.value < rate)
                createIndex = UnityEngine.Random.Range(0, iter.Size);

            do
            {
                Point2D point = iter.Iter;
                Grid grid = GlobalAsset.map.GetAt(point.Binded);

                if(grid != null)
                    updateForObj(grid.Obj);

                if (createIndex-- == 0)
                    animal = CreateAnimal(point.Binded.Copy());

            } while (iter.MoveToNext());
            
            if (energy.IsFull)
                levelUp();

            if (isDead())
                destroy();

            return animal;
        }



        private void levelUp()
        {
            if (level == 10) return;
            ++level;
            energy.MaxExpand(energy.Value);
            Debug.Log("level up " + color.ToString());
            RegisterEvent(ObjEvent.Grow);

            Iterator iter = new Iterator(this.posit, this.level + 1);

            do
            {
                Point2D point = iter.Iter;
                Grid grid = GlobalAsset.map.GetAt(point.Binded);

                if (grid == null || grid.Obj == null)
                    continue;

                if (grid.Obj is Stone && !(grid.Obj is Creater))
                {
                    grid.Obj.RegisterEvent(ObjEvent.Destroy);
                    grid.RemoveObj();
                    Debug.Log("destroy stone");
                }


            } while (iter.MoveToNext());

        }

        private Animal CreateAnimal(Point3D position)
        {
            if (energy.Value < consume*(level+1)) return null;

            Grid grid = GlobalAsset.map.GetAt(position);
            if (grid == null) return null;

            if(grid.InsertObj(new Animal(position, this.color, 10)))
            {
                energy.Add(-consume);
                SkillManager.showSkill(Skill.create, new Point2D(position, GlobalAsset.player.plain.Dimention), Vector2D.Right);
                GlobalAsset.animals.Add((Animal)grid.Obj);
                return (Animal)grid.Obj;
            }

            return null;
        }

        private void updateForObj(MazeObject obj)
        {
            if (obj == null || obj == this) return;

            if(obj is Animal)
            {
                Animal animal = (Animal)obj;
                if (animal.color.Equals(this.color))
                    energy.Add(1);
                else
                    energy.Add(-1);
            }
            else if(obj is Creater)
            {
                Creater creater = (Creater)obj;
                if (creater.color.Equals(this.color))
                    eatCreater(creater);
                else
                    energy.Add(-creater.level);
            }
        }

        private void eatCreater(Creater creater)
        {
            if (this.level <= creater.level) return;

            energy.Add(creater.energy.Value);
            creater.energy.Set(0);
            creater.destroy();
        }

        private void destroy()
        {
            GlobalAsset.map.GetAt(this.position).RemoveObj();
            this.RegisterEvent(ObjEvent.Destroy);
            if (color.Equals(GlobalAsset.player.color))
                Debug.Log("create destroy");
        }
    }
}
