using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opbot.Extensions
{
    public static class QueueExtensions
    {
        public static IEnumerable<T> TakeAndRemove<T>(this Queue<T> queue, int count)
        {
            for (int i = 0; i < Math.Min(queue.Count, count); i++)
                yield return queue.Dequeue();
        }

    }
}
