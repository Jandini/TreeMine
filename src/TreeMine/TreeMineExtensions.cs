using System;
using System.Collections.Generic;

namespace TreeMine
{
    internal static class TreeMineExtensions
    {

        internal static IEnumerable<TSource> LogCount<TSource>(this IEnumerable<TSource> source, int value, Action<int> log)
        {        
            int count = 0;
            
            foreach (var item in source)
            {
                if ((++count % value) == 0)
                    log(count);

                yield return item;
            }

        }
    }
}
