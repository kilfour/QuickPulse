using QuickPulse.Bolts;

namespace QuickPulse;

public static class ToLinq
{
    public static Flow<TValueTwo> Select<TValueOne, TValueTwo>(
        this Flow<TValueOne> runner,
        Func<TValueOne, TValueTwo> selector) =>
            state =>
                {
                    var value = runner(state).Value;
                    if (value == null) return Cask.None<TValueTwo>();
                    return Cask.Some(selector(value));
                };
    public static Flow<TResult> SelectMany<TSource, TResult>(
        this Flow<TSource> source,
        Func<TSource, Flow<TResult>> selector) =>
            state =>
                {
                    var value = source(state).Value;
                    if (value == null) return Cask.None<TResult>();
                    return selector(value)(state);
                };
    public static Flow<TValueThree> SelectMany<TValueOne, TValueTwo, TValueThree>(
        this Flow<TValueOne> runner,
        Func<TValueOne, Flow<TValueTwo>> selector,
        Func<TValueOne, TValueTwo, TValueThree> projector)
            => runner.SelectMany(x => selector(x).Select(y => projector(x, y)));
    public static Flow<T> Where<T>(this Flow<T> source, Func<T, bool> predicate) =>
        state =>
        {
            var result = source(state);
            if (!predicate(result.Value))
                return Cask.None<T>();
            return Cask.Some(result.Value);
        };
}
