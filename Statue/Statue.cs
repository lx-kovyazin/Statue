using System;
using System.Collections.Generic;

namespace Statue
{
    public class Transition<State>
        : Tuple<State, State>
    {
        private Action action;

        private bool IsLoopback { get; }

        public Transition(State start, State end)
            : base(start, end)
        {
            IsLoopback = start.Equals(end);
        }

        public Transition(State loopback)
            : this(loopback, loopback)
        { }

        public Transition<State> With(Action action)
        {
            this.action = action;
            return this;
        }

        internal bool Move(ref State state)
        {
            if (Item1.Equals(state))
            {
                if (!IsLoopback)
                    state = Item2;
                action?.Invoke();
                return true;
            }
            else return false;
        }

        public override string ToString() => $"{Item1} -> {Item2}";
    }

    public class StateMachine<State, Trigger>
    {
        private Dictionary<Trigger, List<Transition<State>>> triggers
            = new Dictionary<Trigger, List<Transition<State>>>();

        private State current;

        public StateMachine(State idle)
        {
            current = idle;
        }

        public State Current => current;

        public void Add(Trigger trigger, Transition<State> transition)
        {
            if (!triggers.ContainsKey(trigger))
                triggers.Add(trigger, new List<Transition<State>>());
            
            triggers[trigger].Add(transition);
        }

        public void Pull(Trigger trigger)
        {
            if (!triggers.ContainsKey(trigger))
                return;
            
            triggers[trigger].Find(t => t.Move(ref current));
        }
    }
}
