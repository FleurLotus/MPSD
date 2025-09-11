///From https://github.com/dotnet/reactive/blob/main/Ix.NET/Source/System.Linq.Async/System/Linq/Operators/ToEnumerable.cs
namespace System.Linq
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static partial class AsyncEnumerable
    {
        // REVIEW: This type of blocking is an anti-pattern. We may want to move it to System.Interactive.Async
        //         and remove it from System.Linq.Async API surface.

        /// <summary>
        /// Converts an async-enumerable sequence to an enumerable sequence.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements in the source sequence.</typeparam>
        /// <param name="source">An async-enumerable sequence to convert to an enumerable sequence.</param>
        /// <returns>The enumerable sequence containing the elements in the async-enumerable sequence.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="source"/> is null.</exception>
        public static IEnumerable<TSource> ToEnumerable<TSource>(this IAsyncEnumerable<TSource> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            return Core(source);

            static IEnumerable<TSource> Core(IAsyncEnumerable<TSource> source)
            {
                IAsyncEnumerator<TSource> e = source.GetAsyncEnumerator(default);

                try
                {
                    while (true)
                    {
                        if (!Wait(e.MoveNextAsync()))
                        {
                            break;
                        }

                        yield return e.Current;
                    }
                }
                finally
                {
                    Wait(e.DisposeAsync());
                }
            }
        }

        // NB: ValueTask and ValueTask<T> do not have to support blocking on a call to GetResult when backed by
        //     an IValueTaskSource or IValueTaskSource<T> implementation. Convert to a Task or Task<T> to do so
        //     in case the task hasn't completed yet.

        private static void Wait(ValueTask task)
        {
            Runtime.CompilerServices.ValueTaskAwaiter awaiter = task.GetAwaiter();

            if (!awaiter.IsCompleted)
            {
                task.AsTask().GetAwaiter().GetResult();
                return;
            }

            awaiter.GetResult();
        }

        private static T Wait<T>(ValueTask<T> task)
        {
            Runtime.CompilerServices.ValueTaskAwaiter<T> awaiter = task.GetAwaiter();

            if (!awaiter.IsCompleted)
            {
                return task.AsTask().GetAwaiter().GetResult();
            }

            return awaiter.GetResult();
        }
    }
}