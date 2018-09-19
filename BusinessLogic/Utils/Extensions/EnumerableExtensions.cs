using System.Collections.Generic;

namespace BusinessLogic.Utils.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Yield<T>(this T item)
        {
            yield return item;
        }
    }
}
