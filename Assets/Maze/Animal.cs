using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    public class Animal : MazeObject,Attackable
    {
        private Point2D posit;
        private Vector2D vect
        { get { return this.plain.Vector3To2(vector); } }
        public Color color;

        public Vector3D vector;
        public EnergyBar hp;
        public EnergyBar ep;
        public EnergyBar hungry;
        public int power;

        public bool isDead
        { get { return this.hp.Value == 0; } }

        public Plain plain
        { get { return posit.Plain; } }

        public Vector2D vectorOnScenen
        { get { return GlobalAsset.player.posit.Plain.Vector3To2(vector); } }


        public Dimention forwardDimen {
            get {
                switch (vector) {
                    case Vector3D.Xn:
                    case Vector3D.Xp:
                        return Dimention.X;

                    case Vector3D.Yn:
                    case Vector3D.Yp:
                        return Dimention.Y;

                    case Vector3D.Zn:
                    case Vector3D.Zp:
                        return Dimention.Z;

                    default:
                        return Dimention.Null;
                }
            }
        }

        public override Color GetColor()
        {
            float a = hp.Value / (hp.Max * 2f) + 0.5f;
            return new Color(color.r, color.g, color.b, a);
        }


        public Animal(Point3D position, Color color, int power) : base(position)
        {
            posit = new Point2D(this.position, Dimention.Z);
            vector = Vector3D.Xp;
            hp = new EnergyBar(100);
            this.color = color;
            this.power = power;
        }

        public override Sprite GetSprite()
        {
            return GlobalAsset.animalShape.GetAt(this.vectorOnScenen);
        }



        public void MoveFor(Vector2D vector)
        {
            if (this.vect == vector)
                Move();
            else
                TurnTo(vector);
        }

        public void ChangePlain()
        {
            ChangePlain(this.forwardDimen);
        }

        public void Auto(int arg)
        {
            int rand = UnityEngine.Random.Range(0, arg);
            switch (rand)
            {
                case 0:
                    MoveFor(Vector2D.Right);
                    break;

                case 1:
                    MoveFor(Vector2D.Down);
                    break;

                case 2:
                    MoveFor(Vector2D.Left);
                    break;

                case 3:
                    MoveFor(Vector2D.Up);
                    break;

                case 4:
                    Attack();
                    break;

                case 5:
                    Straight();
                    break;

                case 6:
                    Horizon();
                    break;

                default:
                    Move();
                    break;
            }
        }



        public void Attack()
        {
            Point3D targetPosition = this.position.Copy();
            SkillManager.showSkill(Skill.attack, PositOnScene, vectorOnScenen);

            targetPosition.MoveFor(this.vector, 1);
            Grid targetGrid = GlobalAsset.map.GetAt(targetPosition);
            if (targetGrid == null) return;
            
            if (targetGrid.Obj == null) return;

            if (targetGrid.Obj is Animal)
            {
                Animal enemy = (Animal)(targetGrid.Obj);
                if (!enemy.color.Equals(this.color))
                    enemy.BeAttack(this);
            }
            else if(targetGrid.Obj is Wall)
            {
                Wall wall = (Wall)targetGrid.Obj;
                wall.BeAttack(this);
            }
        }

        public void Straight()
        {
            Point3D targetPosition = this.position.Copy();
            SkillManager.showSkill(Skill.straight, this.PositOnScene, this.vectorOnScenen);

            for (int i = 0; i < 3; ++i)
            {
                targetPosition.MoveFor(this.vector, 1);
                Grid targetGrid = GlobalAsset.map.GetAt(targetPosition);
                if (targetGrid == null) return;
                
                if (targetGrid.Obj == null) continue;
                if(targetGrid.Obj is Animal){
                    Animal target = (Animal)targetGrid.Obj;
                    if(!target.color.Equals(this.color))
                        target.BeAttack(this);
                }
            }
        }

        public void Horizon()
        {
            Point2D targetPosition = this.posit.Copy();
            Vector2D targetVector = this.vect;

            if(this.plain.Dimention == GlobalAsset.player.plain.Dimention)
                SkillManager.showSkill(Skill.horizon, PositOnScene, vectorOnScenen);
            else
                SkillManager.showSkill(Skill.attack, PositOnScene, vectorOnScenen);

            targetPosition.MoveFor(targetVector, 1);
            targetVector = VectorConvert.Rotate(targetVector);
            targetPosition.MoveFor(targetVector, 2);
            targetVector = VectorConvert.Invert(targetVector);
            
            for(int i=0; i<3; ++i)
            {
                targetPosition.MoveFor(targetVector, 1);

                Grid targetGrid = GlobalAsset.map.GetAt(targetPosition.Binded);
                if (targetGrid == null) continue;
                if (targetGrid.Obj == null) continue;
                if (targetGrid.Obj is Animal)
                {
                    Animal target = (Animal)targetGrid.Obj;
                    if (!target.color.Equals(this.color))
                        target.BeAttack(this);
                }
            }

        }

        public void Build()
        {
            Point3D targetPosition = this.position.Copy();

            targetPosition.MoveFor(this.vector, 1);
            Grid targetGrid = GlobalAsset.map.GetAt(targetPosition);
            if (targetGrid == null) return;

            if (targetGrid.IsEmpty())
            {
                targetGrid.InsertObj(new Wall(targetPosition, 100));
            }
        }



        public void BeAttack(Animal enemy)
        {
            this.hp.Add(-enemy.power);

            if (this.hp.IsZero)
            {
                Grid grid = GlobalAsset.map.GetAt(this.position);
                grid.RemoveObj();
                grid.InsertObj(new Food(this.position, 100));
                RegisterEvent(ObjEvent.Destroy);
            }
        }

        private bool ConsumeEP(int val)
        {
            if (this.ep.Value < val)
                return false;

            this.ep.Add(-val);
            return true;
        }
        
        private void TurnTo(Vector2D vector)
        {
            this.vector = this.plain.Vector2To3(vector);
            RegisterEvent(ObjEvent.shape);
        }

        private void Move()
        {
            Point3D temp = this.position.Copy();
            temp.MoveFor(vector, 1);
            Grid targetGrid = GlobalAsset.map.GetAt(temp);

            if (targetGrid == null)
                return;

            if (targetGrid.Obj != null)
            {
                if (targetGrid.Obj is Food)
                {
                    eatFood((Food)targetGrid.Obj);
                    targetGrid.Obj.RegisterEvent(ObjEvent.Destroy);
                    targetGrid.RemoveObj();
                }
                else
                    return;
            }

            GlobalAsset.map.Swap(position, temp);
            RegisterEvent(ObjEvent.move);
        }

        private void ChangePlain(Dimention dimen)
        {
            this.posit.ChangePlain(dimen);
        }

        private void eatFood(Food food)
        {
            this.hp.Add(food.Nutrient);
        }
        
    }
}
