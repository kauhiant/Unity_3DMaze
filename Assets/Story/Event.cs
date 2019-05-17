using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Story
{
    public class Event
    {
        public delegate bool DelCondition();
        public DelCondition condition;
        public State nextState;
        public Action action;

        public Event(DelCondition condition, State nextState, Action action)
        {
            this.condition = condition;
            this.nextState = nextState;
            this.action = action;
        }

    }

    public abstract class Action
    {
        abstract public void Play();
    }
}
