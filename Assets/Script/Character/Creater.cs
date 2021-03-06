﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    // 生成器(村莊).
    // 可生產村民、食物.
    // 會消除附近的石頭，吸收比自己弱的生成器.
    // 等級越高，可控制範圍越大.
    // 若沒有能量會荒廢(死亡).
    public class Creater : MazeObject
    {
        // 生成器的圖片.
        static private Sprite Sprite;
        static public void SetSprite(Sprite sprite)
        {
            Creater.Sprite = sprite;
        }

        // 給 MapManager 創造更新綁定的 GameObject 用的.
        public override Color GetColor()
        {
            float a = energy.Value / (energy.Max * 2f) + 0.5f;
            return new Color(Color.r, Color.g, Color.b, a);
        }
        public override Vector2 GetScale()
        {
            float scale = Level / 10f + 1;
            return new Vector2(scale, scale);
        }
        public override Sprite GetSprite()
        {
            return Creater.Sprite;
        }

        public override string Name()
        {
            bool isHome = this == GlobalAsset.player.Hometown;
            if (isHome)
                return "你的村莊";
            else
                return "村莊";
        }

        public override string Info()
        {
            bool isFriend = this.Color.Equals(GlobalAsset.player.Color);

            if (isFriend)
                return string.Format("等級 {2}\n能量 {0} / {1}", energy.Value, energy.Max, Level);
            else
                return string.Format("不同顏色是敵人\n但是你沒辦法打他\n等級 {2}\n能量 {0} / {1}", energy.Value, energy.Max, Level);

        }

        public int   Level { get; private set; }
        public Color Color { get; private set; }
        public bool  IsDead
        {
            get
            {
                return energy.IsZero;
            }
        }
        public int Range // 管理範圍.干涉範圍.
        {
            get
            {
                return Level + 1;
            }
        }

        private Point2D positionOnPlain;
        private EnergyBar energy;
        private int costOfAnimal;
        private float rateOfCreateAnimal;
        private float rateOfCreateFood;

        // 平均村民數.
        private int clocks = 0;
        private int animalsCount = 0;// 村民數.
        private float averageOfAnimals = 3; // 平均村民數.

        // 生產食物或村民前要保留一些能量，以免剩餘能量太少.
        private int ReserveEnergy
        {
            get
            {
                return costOfAnimal * (Range);
            }
        }

        public Creater(Point3D position, Color color) : base(position)
        {
            this.positionOnPlain = new Point2D(position, Dimention.Z);
            this.Color = color;
            this.Level = 0;
            this.energy = new EnergyBar(200);
            this.costOfAnimal = 40;
            this.rateOfCreateAnimal = 0.05f;
            this.rateOfCreateFood = 0.1f;

            LevelUp();
        }



        // 每個 clock 執行的事.
        // 若有創造村民，則回傳該村民.
        // 根據機率創造村民、食物.
        // 若能量滿了會升級，能量空了會死亡.
        public Animal Clock()
        {
            if (IsDead) return null;
            reduceForClock();


            Iterator iter = new Iterator(this.positionOnPlain, this.Range);

            int createAnimalIndex = -1;
            if (UnityEngine.Random.value < rateOfCreateAnimal)
                createAnimalIndex = UnityEngine.Random.Range(0, iter.Size);

            int createFoodIndex = -1;
            if (UnityEngine.Random.value < rateOfCreateFood)
                createFoodIndex = UnityEngine.Random.Range(0, iter.Size);
            
            Animal animal = null;
            animalsCount = 0; // count by UpdateByObj().

            do
            {
                Point2D point = iter.Iter;
                Grid grid = World.GetAt(point.Binded);

                if(grid != null)
                    UpdateByObj(grid.Obj);

                if (createAnimalIndex-- == 0)
                    animal = CreateAnimalAt(point.Binded.Copy());

                if (createFoodIndex-- == 0)
                    CreateFoodAt(point.Binded.Copy(), 1);

            } while (iter.MoveToNext());

            averageOfAnimals = (averageOfAnimals * clocks + animalsCount) / (clocks + 1); // 應該是float.

            ++clocks;
            if (clocks > 100)
                clocks = 0; // 避免數字太大溢位.

            if (energy.IsFull)
                LevelUp();
            else if (IsDead)
                Destroy();

            return animal;
        }

        // 升級，最高10級.
        // 會提升能量槽.
        // scale 會變大.
        public void LevelUp()
        {
            if (Level == 10) return;
            ++Level;
            energy.MaxExpand(energy.Value);
            RegisterEvent(ObjEvent.scale);
        }

        // return null   : 創造失敗，能量不夠、格子不存在、格子已經有東西了.
        // return Animal : 創造的生物.
        private Animal CreateAnimalAt(Point3D position)
        {
            if (energy.Value < ReserveEnergy)
                return null;

            Grid grid = World.GetAt(position);
            if (grid == null)
                return null;

            var animal = new Animal(position, this);
            if(grid.InsertObj(animal))
            {
                energy.Add(-costOfAnimal);

                // 生成器等級越高，生物越強.
                int extra = costOfAnimal / 10 * Level;
                energy.Add(-extra);
                animal.Strong(Level * 10, Level, Level);

                SkillManager.showSkill(Skill.create, position);
                GlobalAsset.animals.Add((Animal)grid.Obj);
                return (Animal)grid.Obj;
            }

            return null;
        }

        // 創造營養價值 nutrientBase*20 的食物.
        // false : 創造失敗，能量不夠、格子不存在、格子已經有東西了.
        private bool CreateFoodAt(Point3D position, int nutrientBase)
        {
            if (energy.Value < ReserveEnergy) return false;

            Grid grid = World.GetAt(position);
            if (grid == null) return false;

            if (grid.InsertObj(new Food(position, nutrientBase*20)))
            {
                energy.Add(-nutrientBase);
                return true;
            }

            return false;
        }

        // 若為自己的村民，增加能量.
        // 若為敵人，減少能量.
        // 將同顏色的生物拉為自己村民.
        // 吸收比自己弱的生成器.
        // 消除石頭.
        private void UpdateByObj(MazeObject obj)
        {
            if (obj == null || obj == this) return;

            if(obj is Animal)
            {
                Animal animal = (Animal)obj;
                if (animal.Hometown == this)
                {
                    energy.Add(1);
                    ++animalsCount;
                }
                else if (!animal.Color.Equals(this.Color))
                    energy.Add(-1);
                else
                    animal.ChangeHomeTown(this);
            }
            else if(obj is Creater)
            {
                Creater creater = (Creater)obj;
                if (creater.Color.Equals(this.Color))
                    EatCreater(creater);
                else
                    energy.Add(-creater.Level);
            }
            else if(obj is Stone)
            {
                obj.Destroy();
            }
        }

        // 吸收生成器.
        // false : 自己比對方弱，吸收失敗.
        private bool EatCreater(Creater creater)
        {
            if (this.Level < creater.Level)
                return false;
            else if (this.Level == creater.Level && this.energy.Value < creater.energy.Value)
                return false;

            energy.Add(creater.energy.Value);
            creater.energy.Set(0);
            creater.Destroy();
            return true;
        }
        
        // 每個clock扣多少能量.
        // 依據平均村民數做決定.
        private void reduceForClock()
        {
            int reduce;

            if (averageOfAnimals < 0.125f)
                reduce = 16;
            else if (averageOfAnimals < 0.25f)
                reduce = 8;
            else if (averageOfAnimals < 0.5f)
                reduce = 4;
            else if (averageOfAnimals < 1f)
                reduce = 2;
            else
                reduce = 1;
            
            energy.Add(-reduce);
        }

        public void createAnimals(int number)
        {
            Iterator iter = new Iterator(this.positionOnPlain, 1);

            do
            {
                Point2D point = iter.Iter;
                Grid grid = World.GetAt(point.Binded);

                if (grid == null || !grid.IsEmpty())
                    continue;
                
                if (number-- != 0)
                    CreateAnimalAt(point.Binded.Copy());

            } while (iter.MoveToNext());
        }

        public void FullEnergy()
        {
            this.energy.Add(energy.Max);
        }
    }
}
