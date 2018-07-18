﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    class Animal : MazeObject
    {
        public Point2D posit;
        public Vector3D vector;

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


        private void TurnTo(Vector2D vector)
        {
            this.vector = posit.plain.Vector2To3(vector);
            RegisterEvent("turnTo");
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

            RegisterEvent("move");
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

        private void RegisterEvent(string eventName)
        {
            GlobalAsset.map.GetAt(position).objEvent = eventName;
        }
    }
}