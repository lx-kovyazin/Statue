namespace Statue
{
    public static class Extensions
    {
        public static Transition<State> IntoTransition<State>(this (State start, State end) tuple)
            => new Transition<State>(tuple);

        public static Transition<State> IntoTransition<State>(this State loopback)
            => new Transition<State>(loopback);
    }
}
