using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    public class Animal : MazeObject
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
        { get { return posit.plain; } }

        public Vector2D vectorOnScenen
        { get { return GlobalAsset.player.posit.plain.Vector3To2(vector); } }


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

        public Color GetColor
        {
            get
            {
                float a = hp.Value / (hp.Max * 2f) + 0.5f;
                return new Color(color.r, color.g, color.b, a);
            }
        }

        public Animal(Point3D position, Color color, int power) : base(position)
        {
            posit = new Point2D(this.position, Dimention.Z);
            vector = Vector3D.Xp;
            hp = new EnergyBar(100);
            this.color = color;
            this.power = power;
        }

        public override Sprite Shape()
        {
            return GlobalAsset.animalShape.At(this.vectorOnScenen);
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
            SkillManager.showSkill(Skill.attack, positOnScene, vectorOnScenen);

            targetPosition.MoveFor(this.vector, 1);
            Grid targetGrid = GlobalAsset.map.GetAt(targetPosition);
            if (targetGrid == null) return;
            
            if (targetGrid.obj == null) return;

            if (targetGrid.obj is Animal)
            {
                Animal enemy = (Animal)(targetGrid.obj);
                if (!enemy.color.Equals(this.color))
                    enemy.BeAttack(this);
            }
            else if(targetGrid.obj is Wall)
            {
                Wall wall = (Wall)targetGrid.obj;
                wall.beAttack();
            }
        }

        public void Straight()
        {
            Point3D targetPosition = this.position.Copy();
            SkillManager.showSkill(Skill.straight, this.positOnScene, this.vectorOnScenen);

            for (int i = 0; i < 3; ++i)
            {
                targetPosition.MoveFor(this.vector, 1);
                Grid targetGrid = GlobalAsset.map.GetAt(targetPosition);
                if (targetGrid == null) return;
                
                if (targetGrid.obj == null) continue;
                if(targetGrid.obj is Animal){
                    Animal target = (Animal)targetGrid.obj;
                    if(!target.color.Equals(this.color))
                        target.BeAttack(this);
                }
            }
        }

        public void Horizon()
        {
            Point2D targetPosition = this.posit.Copy();
            Vector2D targetVector = this.vect;

            if(this.plain.dimen == GlobalAsset.player.plain.dimen)
                SkillManager.showSkill(Skill.horizon, positOnScene, vectorOnScenen);
            else
                SkillManager.showSkill(Skill.attack, positOnScene, vectorOnScenen);

            targetPosition.MoveFor(targetVector, 1);
            targetVector = VectorConvert.Rotate(targetVector);
            targetPosition.MoveFor(targetVector, 2);
            targetVector = VectorConvert.Invert(targetVector);
            
            for(int i=0; i<3; ++i)
            {
                targetPosition.MoveFor(targetVector, 1);

                Grid targetGrid = GlobalAsset.map.GetAt(targetPosition.binded);
                if (targetGrid == null) continue;
                if (targetGrid.obj == null) continue;
                if (targetGrid.obj is Animal)
                {
                    Animal target = (Animal)targetGrid.obj;
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

            if (targetGrid.obj == null)
            {
                targetGrid.obj = new Wall(targetPosition);
            }
        }



        private void BeAttack(Animal enemy)
        {
            this.hp.add(-enemy.power);

            if (this.hp.isZero())
            {
                GlobalAsset.map.GetAt(this.position).obj = new Food(this.position, 100);
                RegisterEvent(ObjEvent.Destroy);
            }
        }

        private bool ConsumeEP(int val)
        {
            if (this.ep.Value < val)
                return false;

            this.ep.add(-val);
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

            if (targetGrid.obj != null)
            {
                if (targetGrid.obj is Food)
                {
                    eatFood((Food)targetGrid.obj);
                    targetGrid.obj.RegisterEvent(ObjEvent.Destroy);
                    targetGrid.obj = null;
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
            this.hp.add(food.nutrient);
        }
        
    }
}
