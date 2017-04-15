#region License and Terms
// MoreLINQ - Extensions to LINQ to Objects
// Copyright (c) 2016 Atif Aziz. All rights reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

namespace MoreLinq
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;

    static partial class MoreEnumerable
    {
        /// <summary>
        /// Partitions a grouping by Boolean keys into a projection of true
        /// elements and false elements, respectively.
        /// </summary>
        /// <typeparam name="T">Type of elements in source groupings.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="resultSelector">
        /// Function that projects the result from sequences of true elements
        /// and false elements, respectively, passed as arguments.
        /// </param>
        /// <returns>
        /// The return value from <paramref name="resultSelector"/>.
        /// </returns>

        public static TResult Partition<T, TResult>(this IEnumerable<IGrouping<bool, T>> source,
            Func<IEnumerable<T>, IEnumerable<T>, TResult> resultSelector)
        {
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            return source.Partition(true, false, (t, f, _) => resultSelector(t, f));
        }

        /// <summary>
        /// Partitions a grouping by nullable Boolean keys into a projection of
        /// true elements, false elements and null elements, respectively.
        /// </summary>
        /// <typeparam name="T">Type of elements in source groupings.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="resultSelector">
        /// Function that projects the result from sequences of true elements,
        /// false elements and null elements, respectively, passed as
        /// arguments.
        /// </param>
        /// <returns>
        /// The return value from <paramref name="resultSelector"/>.
        /// </returns>

        public static TResult Partition<T, TResult>(this IEnumerable<IGrouping<bool?, T>> source,
            Func<IEnumerable<T>, IEnumerable<T>, IEnumerable<T>, TResult> resultSelector)
        {
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            return source.Partition(true, false, null, (t, f, n, _) => resultSelector(t, f, n));
        }

        /// <summary>
        /// Partitions a grouping into a projection of elements matching a key
        /// and those groups that do not.
        /// </summary>
        /// <typeparam name="TKey">Type of keys in source groupings.</typeparam>
        /// <typeparam name="TElement">Type of elements in source groupings.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="key">The key to partition.</param>
        /// <param name="resultSelector">
        /// Function that projects the result from sequences of elements
        /// matching <paramref name="key"/> and those groups that do not,
        /// passed as arguments.
        /// </param>
        /// <returns>
        /// The return value from <paramref name="resultSelector"/>.
        /// </returns>

        public static TResult Partition<TKey, TElement, TResult>(this IEnumerable<IGrouping<TKey, TElement>> source,
            TKey key,
            Func<IEnumerable<TElement>, IEnumerable<IGrouping<TKey, TElement>>, TResult> resultSelector) =>
            Partition(source, key, null, resultSelector);

        /// <summary>
        /// Partitions a grouping into a projection of elements matching a key
        /// and those groups that do not. An additional parameter specifies how
        /// to compare keys for equality.
        /// </summary>
        /// <typeparam name="TKey">Type of keys in source groupings.</typeparam>
        /// <typeparam name="TElement">Type of elements in source groupings.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="key">The key to partition.</param>
        /// <param name="comparer">The comparer for keys.</param>
        /// <param name="resultSelector">
        /// Function that projects the result from elements of the group
        /// matching <paramref name="key"/> and those groups that do not,
        /// passed as arguments.
        /// </param>
        /// <returns>
        /// The return value from <paramref name="resultSelector"/>.
        /// </returns>

        public static TResult Partition<TKey, TElement, TResult>(this IEnumerable<IGrouping<TKey, TElement>> source,
            TKey key, IEqualityComparer<TKey> comparer,
            Func<IEnumerable<TElement>, IEnumerable<IGrouping<TKey, TElement>>, TResult> resultSelector)
        {
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            return PartitionImpl(source, 1, key, default(TKey), default(TKey), comparer,
                                 (a, b, c, rest) => resultSelector(a, rest));
        }

        /// <summary>
        /// Partitions a grouping into a projection of elements matching a
        /// set of two keys and those groups that do not.
        /// </summary>
        /// <typeparam name="TKey">Type of keys in source groupings.</typeparam>
        /// <typeparam name="TElement">Type of elements in source groupings.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="key1">The first key to partition.</param>
        /// <param name="key2">The second key to partition.</param>
        /// <param name="resultSelector">
        /// Function that projects the result from elements of the group
        /// matching <paramref name="key1"/>, elements of the group matching
        /// <paramref name="key2"/> and those groups that do not,
        /// passed as arguments.
        /// </param>
        /// <returns>
        /// The return value from <paramref name="resultSelector"/>.
        /// </returns>

        public static TResult Partition<TKey, TElement, TResult>(this IEnumerable<IGrouping<TKey, TElement>> source,
            TKey key1, TKey key2,
            Func<IEnumerable<TElement>, IEnumerable<TElement>, IEnumerable<IGrouping<TKey, TElement>>, TResult> resultSelector) =>
            Partition(source, key1, key2, null, resultSelector);

        /// <summary>
        /// Partitions a grouping into a projection of elements matching a
        /// set of two keys and those groups that do not. An additional
        /// parameter specifies how to compare keys for equality.
        /// </summary>
        /// <typeparam name="TKey">Type of keys in source groupings.</typeparam>
        /// <typeparam name="TElement">Type of elements in source groupings.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="key1">The first key to partition.</param>
        /// <param name="key2">The second key to partition.</param>
        /// <param name="comparer">The comparer for keys.</param>
        /// <param name="resultSelector">
        /// Function that projects the result from elements of the group
        /// matching <paramref name="key1"/>, elements of the group matching
        /// <paramref name="key2"/> and those groups that do not,
        /// passed as arguments.
        /// </param>
        /// <returns>
        /// The return value from <paramref name="resultSelector"/>.
        /// </returns>

        public static TResult Partition<TKey, TElement, TResult>(this IEnumerable<IGrouping<TKey, TElement>> source,
            TKey key1, TKey key2, IEqualityComparer<TKey> comparer,
            Func<IEnumerable<TElement>, IEnumerable<TElement>, IEnumerable<IGrouping<TKey, TElement>>, TResult> resultSelector)
        {
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));
            return PartitionImpl(source, 2, key1, key2, default(TKey), comparer,
                                 (a, b, c, rest) => resultSelector(a, b, rest));
        }

        /// <summary>
        /// Partitions a grouping into a projection of elements matching a
        /// set of three keys and those groups that do not.
        /// </summary>
        /// <typeparam name="TKey">Type of keys in source groupings.</typeparam>
        /// <typeparam name="TElement">Type of elements in source groupings.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="key1">The first key to partition.</param>
        /// <param name="key2">The second key to partition.</param>
        /// <param name="key3">The third key to partition.</param>
        /// <param name="resultSelector">
        /// Function that projects the result from elements of groups
        /// matching <paramref name="key1"/>, <paramref name="key2"/> and
        /// <paramref name="key3"/> and those groups that do not, passed as
        /// arguments.
        /// </param>
        /// <returns>
        /// The return value from <paramref name="resultSelector"/>.
        /// </returns>

        public static TResult Partition<TKey, TElement, TResult>(this IEnumerable<IGrouping<TKey, TElement>> source,
            TKey key1, TKey key2, TKey key3,
            Func<IEnumerable<TElement>, IEnumerable<TElement>, IEnumerable<TElement>, IEnumerable<IGrouping<TKey, TElement>>, TResult> resultSelector) =>
            Partition(source, key1, key2, key3, null, resultSelector);

        /// <summary>
        /// Partitions a grouping into a projection of elements matching a
        /// set of three keys and those groups that do not. An additional
        /// parameter specifies how to compare keys for equality.
        /// </summary>
        /// <typeparam name="TKey">Type of keys in source groupings.</typeparam>
        /// <typeparam name="TElement">Type of elements in source groupings.</typeparam>
        /// <typeparam name="TResult">Type of the result.</typeparam>
        /// <param name="source">The source sequence.</param>
        /// <param name="key1">The first key to partition.</param>
        /// <param name="key2">The second key to partition.</param>
        /// <param name="key3">The third key to partition.</param>
        /// <param name="comparer">The comparer for keys.</param>
        /// <param name="resultSelector">
        /// Function that projects the result from elements of groups
        /// matching <paramref name="key1"/>, <paramref name="key2"/> and
        /// <paramref name="key3"/> and those groups that do not, passed as
        /// arguments.
        /// </param>
        /// <returns>
        /// The return value from <paramref name="resultSelector"/>.
        /// </returns>

        public static TResult Partition<TKey, TElement, TResult>(this IEnumerable<IGrouping<TKey, TElement>> source,
            TKey key1, TKey key2, TKey key3, IEqualityComparer<TKey> comparer,
            Func<IEnumerable<TElement>, IEnumerable<TElement>, IEnumerable<TElement>, IEnumerable<IGrouping<TKey, TElement>>, TResult> resultSelector) =>
            PartitionImpl(source, 3, key1, key2, key3, comparer, resultSelector);

        static TResult PartitionImpl<TKey, TElement, TResult>(IEnumerable<IGrouping<TKey, TElement>> source,
            int count, TKey key1, TKey key2, TKey key3, IEqualityComparer<TKey> comparer,
            Func<IEnumerable<TElement>, IEnumerable<TElement>, IEnumerable<TElement>, IEnumerable<IGrouping<TKey, TElement>>, TResult> resultSelector)
        {
            Debug.Assert(count > 0 && count <= 3);

            if (source == null) throw new ArgumentNullException(nameof(source));
            if (resultSelector == null) throw new ArgumentNullException(nameof(resultSelector));

            comparer = comparer ?? EqualityComparer<TKey>.Default;

            List<IGrouping<TKey, TElement>> etc = null;

            var groups = new[]
            {
                Enumerable.Empty<TElement>(),
                Enumerable.Empty<TElement>(),
                Enumerable.Empty<TElement>(),
            };

            foreach (var e in source)
            {
                var i = count > 0 && comparer.Equals(e.Key, key1) ? 0
                      : count > 1 && comparer.Equals(e.Key, key2) ? 1
                      : count > 2 && comparer.Equals(e.Key, key3) ? 2
                      : -1;

                if (i < 0)
                {
                    etc = etc ?? new List<IGrouping<TKey, TElement>>();
                    etc.Add(e);
                }
                else
                {
                    groups[i] = e;
                }
            }

            return resultSelector(groups[0], groups[1], groups[2], etc ?? Enumerable.Empty<IGrouping<TKey, TElement>>());
        }
    }
}
