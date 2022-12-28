using System;
using System.Collections.Generic;

namespace Statue
{
    public class Statue<Trigger, State>
    {
        private List<Action<StateMachine<Trigger, State>>> defines
            = new List<Action<StateMachine<Trigger, State>>>();

        private Statue() { }

        public static Statue<Trigger, State> Create() => new Statue<Trigger, State>();

        public Statue<Trigger, State> Define(Trigger trigger, params Transition<State>[] transitions)
        {
            defines.Add(sm => sm.AddRange(trigger, transitions));
            return this;
        }

        public StateMachine<Trigger, State> Build(State idle)
        {
            var sm = new StateMachine<Trigger, State>(idle);
            defines.ForEach(add => add(sm));
            return sm;
        }
    }
}
