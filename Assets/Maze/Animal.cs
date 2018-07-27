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
        private Color color;

        public Vector3D vector;
        public int hp;
        public int ep;
        public int hungry;
        public int power;
        public string lastPosition;

        public bool isDead
        { get { return this.hp == 0; } }

        public Plain plain
        { get { return posit.plain; } }

        public Vector2D vectorOnScenen
        { get{return GlobalAsset.player.posit.plain.Vector3To2(vector);} }
        

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
                float a = this.hp / 200f + 0.5f;
                return new Color(color.r, color.g, color.b, a);
            }
        }

        public Animal(Point3D position, Color color) : base(position)
        {
            posit = new Point2D(this.position, Dimention.Z);
            vector = Vector3D.Xp;
            hp = 100;
            this.color = color;
        }

        public override Sprite Shape()
        {
            return GlobalAsset.anamalShape.At(this.vectorOnScenen);
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

                default:
                    Move();
                    break;
            }
        }

        public void Attack()
        {
            Point3D targetPosition = this.position.Copy();
            targetPosition.MoveFor(this.vector, 1);
            Grid targetGrid = GlobalAsset.map.GetAt(targetPosition);
            if (targetGrid == null) return;
            if (targetGrid.obj == null) return;
            if (!(targetGrid.obj is Animal)) return;

            Animal enemy = (Animal) (targetGrid.obj);
            if(!enemy.color.Equals(this.color))
                enemy.BeAttack(10);
        }



        private void BeAttack(int power)
        {
            this.hp -= power;
            if (this.hp <= 0)
            {
                this.hp = 0;
                GlobalAsset.map.GetAt(this.position).obj = null;
                RegisterEvent(ObjEvent.Destroy);
            }
        }

        private bool ConsumeEP(int val)
        {
            if (this.ep < val)
                return false;

            this.ep -= val;
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
                return;

            lastPosition = this.position.ToString();

            GlobalAsset.map.Swap(position, temp);
            RegisterEvent(ObjEvent.move);
        }

        private void ChangePlain(Dimention dimen)
        {
            this.posit.ChangePlain(dimen);
        }


        
    }
}
