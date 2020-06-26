using Microsoft.VisualStudio.TestTools.UnitTesting;
using NMiniKanren;
using System;
using System.Collections.Generic;
using System.Text;

namespace NMiniKanren.Tests
{
    public static class RecDefs
    {
        public static Goal AllOne_Wrong(this KRunner k, object lst)
        {
            var d = k.Fresh();
            return k.Any(
                k.Eq(lst, null),
                k.All(
                    k.Eq(lst, k.Pair(1, d)),
                    k.AllOne_Wrong(d)));
        }
        public static Goal AllOne(this KRunner k, object lst)
        {
            var d = k.Fresh();
            return k.Any(
                k.Eq(lst, null),
                k.All(
                    k.Eq(lst, k.Pair(1, d)),
                    k.Recurse(() => k.AllOne(d))));
        }
    }

    [TestClass]
    public class RecTests
    {

        [TestMethod]
        public void TestAll1()
        {
            KRunner.PrintResult(KRunner.Run(null, (k, q) =>
            {
                var x = k.Fresh();
                var y = k.Fresh();
                return k.All(
                    k.AllOne(k.List(1, x, y, 1)),
                    k.Eq(q, k.List(x, y)));
            }));  // 输出[(1 1)]
            KRunner.PrintResult(KRunner.Run(null, (k, q) =>
            {
                var x = k.Fresh();
                var y = k.Fresh();
                return k.All(
                    k.AllOne(k.List(1, x, y, 0)),
                    k.Eq(q, k.List(x, y)));
            }));  // 输出[]
        }

        [TestMethod]
        public void TestMembero()
        {
            KRunner.PrintResult(KRunner.Run(null, (k, q) =>
            {
                return k.All(
                    k.Membero(q, k.List("a", "b", "c")),
                    k.Membero(q, k.List("a", "c", "f")));
            }));
        }
    }
}
