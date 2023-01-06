# About
The project is a simple library for building state machines easily.


# Getting started

### For example, you need to write a [`light switch`](https://flaviocopes.com/finite-state-machines/) model.

First of all, you should have to define states and triggers, for example using enums.
```C#
enum Light { Off, Red, Yellow, Green }
enum Trigger { TurnOn, TurnOff, Switch }
```

After that, you can build a state machine using `Statue`.
```C#
var stateMachine = Statue<Trigger, Light>
    // Create a builder instance.
    .Create()
    // Connect one Trigger to single or multiple Transitions.
    .Define(
        Trigger.TurnOn,
        // You can use a constructor directly..
        new Transition<Light>(Light.Off, Light.Red).With(() => WriteLine("Turned on"))
    )
    .Define(
        Trigger.TurnOff,
        // .. or extension methods on tuples or values.
        (Light.Red, Light.Off).IntoTransition(),
        (Light.Yellow, Light.Off).IntoTransition(),
        (Light.Green, Light.Off).IntoTransition()
    )
    .Define(
        Trigger.Switch,
        (Light.Red, Light.Yellow)
            .IntoTransition()
            // Also you can connect an action to a transition
            // that must be called while transition performing.
            .With(() => WriteLine("Yellow light on")),
        (Light.Yellow, Light.Green).IntoTransition().With(() => WriteLine("Green light on")),
        (Light.Green, Light.Red).IntoTransition().With(() => WriteLine("Red light on"))
    )
    // After all, build this state machine with the initial state.
    .Build(Light.Off);

```

Next, you could write traffic light model, that something like this:

```C#
public class TrafficLight
{
    private enum Trigger { TurnOn, TurnOff, Switch }
    public enum Light { Off, Red, Yellow, Green }

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

    // The method Pull is used to initiate a transition,
    // that was registered on specific trigger, depends on a current state.
    public void TurnOn() => stateMachine.Pull(Trigger.TurnOn);
    public void TurnOff() => stateMachine.Pull(Trigger.TurnOff);
    public void Switch() => stateMachine.Pull(Trigger.Switch);
}
```

Fine! Let's test it with [`xUnit`](https://xunit.net/).

```C#
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
```
### Congratulations!

Take a look on more examples [`here`](https://github.com/lx-kovyazin/Statue/tree/master/StatueTest/Scenario).
