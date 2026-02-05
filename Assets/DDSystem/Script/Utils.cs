using System;
using System.Collections.Generic;
using System.Linq;

public static class CollectionExtensions
{
    public static IEnumerable<T> Randomize<T>(this IEnumerable<T> source)
    {
        Random rnd = new Random();
        if (source.Count() == 0)
        {
            return Enumerable.Empty<T>();
        }
        return source.OrderBy(_ => rnd.Next());
    }
}