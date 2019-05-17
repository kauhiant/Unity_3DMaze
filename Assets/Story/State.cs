using System.Collections;
using System.Collections.Generic;


namespace Story
{
    public class State
    {
        public List<Event> events = new List<Event>();

        public State AddEvent(Event e)
        {
            events.Add(e);
            return this;
        }
    }

}
