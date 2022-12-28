using System.Collections;
using Statue;

namespace StatueTest.Scenario;

public static class Helper
{
    public static bool TryGetCurrent<T>(this IEnumerator instance, out T? current)
    {
        try
        {
            current = (T)instance.Current;
        }
        catch (Exception) // InvalidOperationException or InvalidCastException
        {
            current = default(T);
            return false;
        }
        return true;
    }
}

public class CombinationLock
{
    enum State
    {
        Locked,
        Entering,
        Opened,
        Failed,
    }
    enum Trigger
    {
        Lock,
        Unlock,
        EnterDigit,
    }

    private readonly IEnumerator cit;
    private readonly StateMachine<Trigger, State> sm;

    public string Status { get; private set; } = string.Empty;

    public CombinationLock(int[] combination)
    {
        this.cit = combination.GetEnumerator();

        sm = Statue<Trigger, State>
            .Create()
            .Define(
               Trigger.Lock,
               (State.Entering, State.Failed).IntoTransition().With(() => Status = nameof(State.Failed).ToUpper()),
               (State.Failed, State.Locked).IntoTransition(),
               (State.Opened, State.Locked).IntoTransition(),
               State.Locked.IntoTransition()
            )
            .Define(
                Trigger.EnterDigit,
                State.Entering.IntoTransition(),
                (State.Locked, State.Entering).IntoTransition().With(() =>
                {
                    Status = "";
                    cit.MoveNext();
                })
            )
            .Define(
               Trigger.Unlock,
               (State.Entering, State.Opened).IntoTransition().With(() => Status = nameof(State.Opened).ToUpper())
            )
            .Build(State.Locked);

        Lock();
    }

    private void Unlock()
    {
        sm.Pull(Trigger.Unlock);
    }

    public void Lock() {
        Status = nameof(State.Locked).ToUpper();
        cit.Reset();
        sm.Pull(Trigger.Lock);
    }

    public void EnterDigit(int digit)
    {
        sm.Pull(Trigger.EnterDigit);
        int current;
        if (cit.TryGetCurrent(out current) && current == digit)
        {
            Status += digit.ToString();
            if (!cit.MoveNext())
                Unlock();
        }
        else Lock();
    }
}

public class CombinationLockTest
{
    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, "LOCKED", "OPENED")]
    public CombinationLock NormalWork(int[] combination, string init, string last)
    {
        var cl = new CombinationLock(combination);
        Assert.Equal(init, cl.Status);

        var actual = combination[..^1].Select(d => {
                cl.EnterDigit(d);
                return d.ToString();
            })
            .Aggregate(String.Concat);
        Assert.Equal(actual, cl.Status);

        cl.EnterDigit(combination[^1]);
        Assert.Equal(last, cl.Status);

        return cl;
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, "LOCKED")]
    public void NormalWorkButLockAfter(int[] combination, string status)
    {
        var cl = NormalWork(combination, status, "OPENED");
        cl.Lock();
        Assert.Equal(status, cl.Status);
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, new[] { 1, 5 }, "FAILED")]
    public CombinationLock IncorrectCombinationEntered(int[] combination, int[] digits, string status)
    {
        var cl = new CombinationLock(combination);
        cl.EnterDigit(digits[0]);
        cl.EnterDigit(digits[^1]);
        Assert.Equal(status, cl.Status);
        return cl;
    }

    [Theory]
    [InlineData(new[] { 1, 2, 3, 4, 5 }, new[] { 1, 5 }, "LOCKED")]
    public void IncorrectCombinationEnteredButLockAfter(int[] combination, int[] digits, string status)
    {
        var cl = IncorrectCombinationEntered(combination, digits, "FAILED");
        cl.Lock();
        Assert.Equal(status, cl.Status);
    }

    [Fact]
    public void FailedOnEmptyCombination()
    {
        var cl = new CombinationLock(Array.Empty<int>());
        cl.EnterDigit(1);
        Assert.Equal("FAILED", cl.Status);
    }
}