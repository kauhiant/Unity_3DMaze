using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Maze
{
    public class Shape
    {
        private Sprite[] eachVector;

        /// <summary>
        /// sprites.index: right, down, left, up, in, out
        /// </summary>
        /// <param name="sprites"></param>
        public Shape(Sprite[] sprites)
        {
            eachVector = new Sprite[6];
            SetAt(Vector2D.Right, sprites[0]);
            SetAt(Vector2D.Down, sprites[1]);
            SetAt(Vector2D.Left, sprites[2]);
            SetAt(Vector2D.Up, sprites[3]);
            SetAt(Vector2D.In, sprites[4]);
            SetAt(Vector2D.Out, sprites[5]);
        }


        public void SetAt(Vector2D vector, Sprite sprite)
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

        public Sprite GetAt(Vector2D vector)
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
