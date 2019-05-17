using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Story
{
    public class StoryManager
    {
        public State currentState;

        public void Clock()
        {
            if (currentState == null)
                return;

            foreach(var e in currentState.events)
            {
                if (e.condition())
                {
                    e.action.Play();
                    currentState = e.nextState;
                    break;
                }
            }
        }

    }
    
}
