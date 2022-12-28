using System;

namespace Statue
{
    // TODO: Make it serializable.

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

        public Transition((State start, State end) tuple)
            : this(tuple.start, tuple.end)
        { }

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
}
