using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NMiniKanren
{
    public delegate Stream DelayedStream();

    public class Stream
    {
        public Substitution Curr { get; set; }
        public DelayedStream GetRest { get; set; }

        private Stream() { }

        private static readonly Stream theEmptyStream = new Stream();

        public bool IsEmpty()
        {
            return this == theEmptyStream;
        }

        public static Stream Empty()
        {
            return theEmptyStream;
        }

        private static Stream MakeStream(Substitution curr, DelayedStream getRest)
        {
            return new Stream()
            {
                Curr = curr,
                GetRest = getRest
            };
        }

        public static Stream Unit(Substitution sub)
        {
            return MakeStream(sub, () => Empty());
        }

        public Stream Append(DelayedStream f)
        {
            if (IsEmpty()) return f();
            return MakeStream(Curr, () => GetRest().Append(f));
        }

        public Stream Interleave(DelayedStream f)
        {
            if (IsEmpty()) return f();
            return MakeStream(Curr, () => f().Interleave(GetRest));
        }

        public Stream Bind(Goal g)
        {
            if (IsEmpty()) return Empty();
            return g(Curr).Append(() => GetRest().Bind(g));
        }

        public Stream Bindi(Goal g)
        {
            if (IsEmpty()) return Empty();
            return g(Curr).Interleave(() => GetRest().Bindi(g));
        }

        public IList<T> MapAndTake<T>(int? n, Func<Substitution, T> mapper)
        {
            if (n == 0 || IsEmpty()) return Enumerable.Empty<T>().ToList();
            var item = mapper(Curr);
            var lst = GetRest().MapAndTake(n.HasValue ? n - 1 : null, mapper);
            lst.Insert(0, item);
            return lst;
        }

    }
}
