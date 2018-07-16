using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    class Shape
    {
        private Sprite[] eachVector;

        public Shape()
        {
            eachVector = new Sprite[6];
        }

        public Shape Copy()
        {
            Shape temp = new Shape();
            return temp;
        }

        public void SetShapeAt(Vector2D vector, Sprite sprite)
        {
            switch (vector)
            {
                case Vector2D.Up:   eachVector[0] = sprite;
                    break;

                case Vector2D.Down: eachVector[1] = sprite;
                    break;

                case Vector2D.Left: eachVector[2] = sprite;
                    break;

                case Vector2D.Right:eachVector[3] = sprite;
                    break;

                case Vector2D.In:   eachVector[4] = sprite;
                    break;

                case Vector2D.Out:  eachVector[5] = sprite;
                    break;
            }
        }

        public Sprite At(Vector2D vector)
        {
            switch (vector)
            {
                case Vector2D.Up:
                    return eachVector[0];

                case Vector2D.Down:
                    return eachVector[1];

                case Vector2D.Left:
                    return eachVector[2];

                case Vector2D.Right:
                    return eachVector[3];

                case Vector2D.In:
                    return eachVector[4];

                case Vector2D.Out:
                    return eachVector[5];

                default:
                    return null;
            }
        }
    }
}
