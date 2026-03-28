namespace System.Linq
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public static partial class AsyncEnumerable
    {
        public static async Task<TSource[]> ToArrayAsync<TSource>(this IAsyncEnumerable<TSource> source, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(source);

            List<TSource> list = new List<TSource>();
            await foreach (TSource item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
            {
                list.Add(item);
            }

            return list.ToArray();
        }
    }
}