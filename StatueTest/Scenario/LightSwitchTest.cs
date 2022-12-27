using Statue;

namespace StatueTest.Scenario
{
    public class LightSwitchTest
    {
        private enum State { Off, On }
        private enum Trigger { Switch }

        [Fact]
        public void OffIdleOffOnOff()
        {
            var sm = new StateMachine<State, Trigger>(State.Off);
            sm.Add(Trigger.Switch, new(State.Off, State.On));
            sm.Add(Trigger.Switch, new(State.On, State.Off));

            Assert.Equal(State.Off, sm.Current);
            sm.Pull(Trigger.Switch);
            Assert.Equal(State.On, sm.Current);
            sm.Pull(Trigger.Switch);
            Assert.Equal(State.Off, sm.Current);
        }

        [Fact]
        public void OffIdleOffOnOffBackwards()
        {
            var sm = new StateMachine<State, Trigger>(State.Off);
            sm.Add(Trigger.Switch, new(State.On, State.Off));
            sm.Add(Trigger.Switch, new(State.Off, State.On));

            Assert.Equal(State.Off, sm.Current);
            sm.Pull(Trigger.Switch);
            Assert.Equal(State.On, sm.Current);
            sm.Pull(Trigger.Switch);
            Assert.Equal(State.Off, sm.Current);
        }

        [Fact]
        public void OnIdleOffOnOff()
        {
            var sm = new StateMachine<State, Trigger>(State.On);
            sm.Add(Trigger.Switch, new(State.On, State.Off));
            sm.Add(Trigger.Switch, new(State.Off, State.On));

            Assert.Equal(State.On, sm.Current);
            sm.Pull(Trigger.Switch);
            Assert.Equal(State.Off, sm.Current);
            sm.Pull(Trigger.Switch);
            Assert.Equal(State.On, sm.Current);
        }

        [Fact]
        public void OnIdleOffOnOffBackwards()
        {
            var sm = new StateMachine<State, Trigger>(State.On);
            sm.Add(Trigger.Switch, new(State.Off, State.On));
            sm.Add(Trigger.Switch, new(State.On, State.Off));

            Assert.Equal(State.On, sm.Current);
            sm.Pull(Trigger.Switch);
            Assert.Equal(State.Off, sm.Current);
            sm.Pull(Trigger.Switch);
        }
    }
}
