using System;
using System.Collections.Generic;

namespace HotLib.DotNetExtensions
{
    /// <summary>
    /// Contains public static extension methods for <see cref="Queue{T}"/>.
    /// </summary>
    public static class QueueExtensions
    {
        /// <summary>
        /// Enqueues a range of items in the queue.
        /// </summary>
        /// <typeparam name="T">The type of item in the queue.</typeparam>
        /// <param name="queue">The queue being appended.</param>
        /// <param name="items">An enumerable of items to add.</param>
        /// <exception cref="ArgumentNullException"><paramref name="queue"/> or <paramref name="items"/> is null.</exception>
        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            if (queue == null)
                throw new ArgumentNullException(nameof(queue));
            if (items == null)
                throw new ArgumentNullException(nameof(items));
            foreach (var item in items)
                queue.Enqueue(item);
        }
    }
}
