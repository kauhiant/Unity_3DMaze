using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Maze
{
    public class EnergyBar
    {
        private int max;
        private int value;

        public int Max { get { return max; } }
        public int Value { get { return value; } }

        public EnergyBar(int max, float rate = 1f)
        {
            this.max = max;
            this.value = (int)(max*rate);
        }

        public void add(int value)
        {
            this.value += value;

            if (this.value < 0)
                this.value = 0;

            if (this.value > max)
                this.value = max;
        }

        public bool isZero()
        {
            return value == 0;
        }

        public bool isFull()
        {
            return value == max;
        }

        public void set(int value)
        {
            this.value = value;
            if (this.value > max)
                this.value = max;
        }

        public void maxExpand(int value)
        {
            this.max += value;
        }

        // return [0.0 , 1.0]
        public float barRate()
        {
            return (float)value / (float)max;
        }
    }
}
