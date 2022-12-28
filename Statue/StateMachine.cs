using System;
using System.Linq;
using System.Collections.Generic;

namespace Statue
{
    // TODO: Make it serializable.

    public class StateMachine<Trigger, State>
    {
        private Dictionary<Trigger, HashSet<Transition<State>>> triggers
            = new Dictionary<Trigger, HashSet<Transition<State>>>();

        private State current;

        public StateMachine(State idle)
        {
            current = idle;
        }

        public State Current => current;

        public void Add(Trigger trigger, Transition<State> transition)
        {
            if (!triggers.ContainsKey(trigger))
                triggers.Add(trigger, new HashSet<Transition<State>>());
            
            triggers[trigger].Add(transition);
        }

        public void AddRange(Trigger trigger, IEnumerable<Transition<State>> transitions)
        {
            if (triggers.ContainsKey(trigger))
                throw new InvalidOperationException("Multiple triggers are not allowed.");
            
            var ts = transitions.ToList();
            if (ts.Count == 0)
                throw new InvalidOperationException("The trigger has no transitions.");

            ts.ForEach(transition => Add(trigger, transition));
        }

        public Transition<State> Pull(Trigger trigger)
        {
            if (!triggers.ContainsKey(trigger))
                return null;
            
            return triggers[trigger].ToList().Find(t => t.Move(ref current));
        }
    }
}
