using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DirectoryNavigator
{
    internal static class DirectoryTreeExtensions
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
