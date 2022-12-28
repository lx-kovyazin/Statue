using Statue;
using static System.Console;

namespace StatueTest.Scenario
{
    public class TrafficLight
    {
        public enum Light { Off, Red, Yellow, Green }
        enum Trigger { TurnOn, TurnOff, Switch }

        private readonly StateMachine<Trigger, Light> stateMachine;

        public Light State => stateMachine.Current;

        public TrafficLight(Light initial)
        {
            stateMachine = Statue<Trigger, Light>
                .Create()
                .Define(
                    Trigger.TurnOn,
                    new Transition<Light>(Light.Off, initial).With(() => WriteLine("Turned on"))
                )
                .Define(
                    Trigger.TurnOff,
                    (Light.Red, Light.Off).IntoTransition(),
                    (Light.Yellow, Light.Off).IntoTransition(),
                    (Light.Green, Light.Off).IntoTransition()
                )
                .Define(
                    Trigger.Switch,
                    (Light.Red, Light.Yellow).IntoTransition().With(() => WriteLine("Yellow light on")),
                    (Light.Yellow, Light.Green).IntoTransition().With(() => WriteLine("Green light on")),
                    (Light.Green, Light.Red).IntoTransition().With(() => WriteLine("Red light on"))
                )
                .Build(Light.Off);
        }

        public void TurnOn() => stateMachine.Pull(Trigger.TurnOn);
        public void TurnOff() => stateMachine.Pull(Trigger.TurnOff);
        public void Switch() => stateMachine.Pull(Trigger.Switch);
    }

    public class TrafficLightTest
    {
        [Fact]
        public void NormalWork()
        {
            var tl = new TrafficLight(TrafficLight.Light.Red);
            Assert.Equal(TrafficLight.Light.Off, tl.State);
            tl.TurnOn();
            Assert.Equal(TrafficLight.Light.Red, tl.State);
            tl.Switch();
            Assert.Equal(TrafficLight.Light.Yellow, tl.State);
            tl.Switch();
            Assert.Equal(TrafficLight.Light.Green, tl.State);
            tl.TurnOff();
            Assert.Equal(TrafficLight.Light.Off, tl.State);
        }
    }
}