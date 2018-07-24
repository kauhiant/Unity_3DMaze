using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    public class Animal : MazeObject
    {
        public Point2D posit;
        public Vector3D vector;
        public int hp;
        public int ep;
        public int hungry;
        public int power;

        public bool isDead { get { return this.hp == 0; } }
        public Plain plain { get { return posit.plain; } }
        public Vector2D vect { get { return plain.Vector3To2(vector); } }
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

        public Animal(Point3D position) : base(position)
        {
            posit = new Point2D(this.position, Dimention.Z);
            vector = Vector3D.Xp;
            hp = 100;
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

        public void BeAttack(int power)
        {
            this.hp -= power;
            if (this.hp <= 0)
            {
                this.hp = 0;
                GlobalAsset.map.GetAt(this.position).obj = null;
            }
                
        }

        public void Auto()
        {
            int rand = UnityEngine.Random.Range(0, 10);
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
            enemy.BeAttack(100);
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
            if (this.vect == vector)
                return;

            this.vector = posit.plain.Vector2To3(vector);
            RegisterEvent(ObjEvent.shape);
        }

        private void Move()
        {
            Point2D temp = this.posit.Copy();
            temp.binded.MoveFor(vector, 1);
            Grid targetGrid = GlobalAsset.map.GetAt(temp.binded);

            if (targetGrid == null)
            {
                return;
            }


            if (targetGrid.obj != null)
            {
                return;
            }

            RegisterEvent(ObjEvent.move);
            GlobalAsset.map.Swap(position, temp.binded);

            return;
        }

        private void ChangePlain(Dimention dimen)
        {
            this.posit.ChangePlain(dimen);
        }

        public override Sprite Shape()
        {
            return GlobalAsset.anamalShape.At(this.vect);
        }

        private void RegisterEvent(ObjEvent eventName)
        {
            GlobalAsset.map.GetAt(position).objEvent = eventName;
        }
    }
}
