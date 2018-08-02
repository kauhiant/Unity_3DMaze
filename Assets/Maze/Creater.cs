using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    public class Creater : MazeObject
    {
        private Point2D posit;
        private Color color;
        private int level;
        private EnergyBar energy;
        private int consume;
        private float rateOdAnimal;
        private float rateOfFood;

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
            this.consume = 40;
            this.rateOdAnimal = 0.05f;
            this.rateOfFood = 0.1f;

            LevelUp();
        }

        public override Sprite GetSprite()
        {
            return GlobalAsset.createrSprite;
        }

        public bool IsDead()
        {
            return energy.IsZero;
        }

        public Animal Clock()
        {
            if (IsDead()) return null;
            energy.Add(-1);

            Iterator iter = new Iterator(this.posit, this.level+1);

            int createAnimalIndex = -1;
            Animal animal = null;
            if (UnityEngine.Random.value < rateOdAnimal)
                createAnimalIndex = UnityEngine.Random.Range(0, iter.Size);

            int createFoodIndex = -1;
            if (UnityEngine.Random.value < rateOfFood)
                createFoodIndex = UnityEngine.Random.Range(0, iter.Size);

            do
            {
                Point2D point = iter.Iter;
                Grid grid = GlobalAsset.map.GetAt(point.Binded);

                if(grid != null)
                    updateByObj(grid.Obj);

                if (createAnimalIndex-- == 0)
                    animal = CreateAnimal(point.Binded.Copy());

                if (createFoodIndex-- == 0)
                    CreateFood(point.Binded.Copy(), 1);

            } while (iter.MoveToNext());
            
            if (energy.IsFull)
                LevelUp();

            if (IsDead())
                destroy();

            return animal;
        }



        private void LevelUp()
        {
            if (level == 10) return;
            ++level;
            energy.MaxExpand(energy.Value);
            RegisterEvent(ObjEvent.Grow);

            Iterator iter = new Iterator(this.posit, this.level + 1);

            do
            {
                Point2D point = iter.Iter;
                Grid grid = GlobalAsset.map.GetAt(point.Binded);

                if (grid == null || grid.Obj == null)
                    continue;

                if (grid.Obj is Stone)
                {
                    grid.Obj.RegisterEvent(ObjEvent.Destroy);
                    grid.RemoveObj();
                }
                else if(grid.Obj is Animal)
                {
                    Animal animal = (Animal)grid.Obj;
                    if ((animal.Color.Equals(this.color)))
                        animal.ChangeHomeTown(this);
                }


            } while (iter.MoveToNext());

        }

        private Animal CreateAnimal(Point3D position)
        {
            if (energy.Value < consume*(level+1)) return null;

            Grid grid = GlobalAsset.map.GetAt(position);
            if (grid == null) return null;

            if(grid.InsertObj(new Animal(position, this, 20)))
            {
                energy.Add(-consume);
                SkillManager.showSkill(Skill.create, new Point2D(position, GlobalAsset.player.plain.Dimention), Vector2D.Right);
                GlobalAsset.animals.Add((Animal)grid.Obj);
                return (Animal)grid.Obj;
            }

            return null;
        }

        private void CreateFood(Point3D position, int nutrientBase)
        {
            if (energy.Value < consume * (level + 1)) return;

            Grid grid = GlobalAsset.map.GetAt(position);
            if (grid == null) return;

            if (grid.InsertObj(new Food(position, nutrientBase*20)))
            {
                energy.Add(-nutrientBase);
            }
        }

        private void updateByObj(MazeObject obj)
        {
            if (obj == null || obj == this) return;

            if(obj is Animal)
            {
                Animal animal = (Animal)obj;
                if (animal.Color.Equals(this.color))
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
        }
    }
}
