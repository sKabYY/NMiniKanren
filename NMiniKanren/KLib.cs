using System;
using System.Collections.Generic;
using System.Text;

namespace NMiniKanren
{
    public static class KLib
    {
        public static Goal Lhso(this KRunner k, object l, object a)
        {
            var d = k.Fresh();
            return k.Eq(k.Pair(a, d), l);
        }

        public static Goal Rhso(this KRunner k, object l, object d)
        {
            var a = k.Fresh();
            return k.Eq(k.Pair(a, d), l);
        }

        public static Goal Membero(this KRunner k, object x, object l)
        {
            var d = k.Fresh();
            return k.Any(
                k.Lhso(l, x),
                k.All(
                    k.Rhso(l, d),
                    k.Recurse(() => Membero(k, x, d))));
        }
    }
}
