namespace Common;

/// <summary>Functional extensions for generic service work.</summary>
public static class FunctionalExtensions
{
    /// <summary>Runs work fn with service TSource returning TResult.</summary>
    /// <typeparam name="TSource">Service</typeparam>
    /// <typeparam name="TResult">Result</typeparam>
    /// <param name="this">Service invoker</param>
    /// <param name="fn">Work</param>
    public static TResult Map<TSource, TResult>(this TSource @this, Func<TSource, TResult> fn)
        => fn(@this);

    /// <summary>Runs work fn with service TSource returning the same service.</summary>
    public static T Tee<T>(this T @this, Action<T> fn)
    {
        fn(@this);
        return @this;
    }
}

public static class Printer
{
    public static object Print(this object o)
    {
        Console.WriteLine($"{o}");
        return o;
    }
}