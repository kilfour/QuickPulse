using QuickPulse.Bolts;

namespace QuickPulse;

public static class ToLinq
{
    public static Flow<TValueTwo> Select<TValueOne, TValueTwo>(
        this Flow<TValueOne> runner,
        Func<TValueOne, TValueTwo> selector) =>
        state =>
        {
            var result = runner(state);
            return Cask.Some(result.state, selector(result.Value));
        };

    public static Flow<TResult> SelectMany<TSource, TResult>(
        this Flow<TSource> source,
        Func<TSource, Flow<TResult>> selector) =>
        state =>
        {
            var result = source(state);
            if (Cask.IsNone(result)) return Cask.None<TResult>(result.state);
            return selector(result.Value)(result.state);
        };

    public static Flow<TValueThree> SelectMany<TValueOne, TValueTwo, TValueThree>(
        this Flow<TValueOne> runner,
        Func<TValueOne, Flow<TValueTwo>> selector,
        Func<TValueOne, TValueTwo, TValueThree> projector)
            => runner.SelectMany(x => selector(x).Select(y => projector(x, y)));

    // public static Flow<T> Where<T>(this Flow<T> source, Func<T, bool> predicate) =>
    //     state =>
    //     {
    //         var result = source(state);
    //         if (Cask.IsNone(result) || !predicate(result.Value))
    //             return Cask.None<T>(result.state);
    //         return Cask.Some(result.state, result.Value);
    //     };
}
