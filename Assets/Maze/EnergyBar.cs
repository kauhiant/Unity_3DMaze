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

        public EnergyBar(int max)
        {
            this.max = max;
            this.value = max;
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
    }
}
