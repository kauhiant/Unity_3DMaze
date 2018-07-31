using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    public class EnergyBar
    {
        public int Max      { get; private set; }
        public int Value    { get; private set; }
        
        public bool IsZero
        {
            get
            {
                return Value == 0;
            }
        }

        public bool IsFull
        {
            get
            {
                return Value == Max;
            }
        }

        /// <summary>
        /// return [0f - 1f]
        /// </summary>
        /// <returns></returns>
        public float BarRate
        {
            get
            {
                return (float)Value / (float)Max;
            }
        }


        public EnergyBar(int max, float rate = 1f)
        {
            this.Max    = max;
            this.Value  = (int)(max * rate);
        }


        /// <summary>
        /// add or reduce value , and not out of range[ 0 , max ].
        /// </summary>
        /// <param name="value"></param>
        public void Add(int value)
        {
            this.Value += value;

            if (this.Value < 0)
                this.Value = 0;

            if (this.Value > Max)
                this.Value = Max;
        }

        /// <summary>
        /// set value, and it not out of range [ 0 , max ].
        /// </summary>
        /// <param name="value"></param>
        public void Set(int value)
        {
            this.Value = value;

            if (this.Value > Max)
                this.Value = Max;

            if (this.Value < 0)
                this.Value = 0;
        }

        /// <summary>
        /// expand max, and no change value
        /// </summary>
        /// <param name="value"></param>
        public void MaxExpand(int value)
        {
            this.Max += value;
        }

    }
}
