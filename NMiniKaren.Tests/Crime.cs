using Microsoft.VisualStudio.TestTools.UnitTesting;
using NMiniKanren;
using System;
using System.Linq;

namespace NMiniKanren.Tests
{
    [TestClass]
    public class Crime
    {
        [TestMethod]
        public void FindMurderer()
        {
            var George = "George";
            var John = "John";
            var Rebert = "Rebert";
            var Barbara = "Barbara";
            var Christine = "Christine";
            var Yolanda = "Yolanda";
            var res = KRunner.Run(10, (k, q) =>
            {
                // 男人集合
                var manNames = new string[] { George, John, Rebert };
                var man = k.List(manNames);
                // 女人集合
                var womanNames = new string[] { Barbara, Christine, Yolanda };
                var woman = k.List(womanNames);
                // 所有人集合
                var person = k.List(manNames.Concat(womanNames).ToArray());
                // 每个场所所在的人
                var bathroom = k.Fresh();
                var dining = k.Fresh();
                var kitchen = k.Fresh();
                var livingroom = k.Fresh();
                var pantry = k.Fresh();
                var study = k.Fresh();
                // 物品持有者
                var bag = k.Fresh();
                var firearm = k.Fresh();
                var gas = k.Fresh();
                var knife = k.Fresh();
                var poison = k.Fresh();
                var rope = k.Fresh();
                // 不同的人在不同的房间
                var locationConst = k.Distincto(bathroom, dining, kitchen, livingroom, pantry, study);
                // 不同的人持有的物品不同
                var weaponConst = k.Distincto(bag, firearm, gas, knife, poison, rope);
                // 变量X表示凶手
                var X = k.Fresh();
                // 线索
                var clue1 = k.All(
                    k.Is(kitchen, man),
                    k.Noto(k.Eq(kitchen, rope), k.Eq(kitchen, knife), k.Eq(kitchen, bag), k.Eq(kitchen, firearm)));
                var clue2 = k.Any(
                    k.All(k.Eq(bathroom, Barbara), k.Eq(study, Yolanda)),
                    k.All(k.Eq(bathroom, Yolanda), k.Eq(study, Barbara)));
                var clue3 = k.Noto(
                    k.Eq(bag, Barbara), k.Eq(bag, George),
                    k.Eq(bag, bathroom), k.Eq(bag, dining));
                var clue4 = k.All(k.Is(rope, woman), k.Eq(rope, study));
                var clue5 = k.Any(k.Eq(livingroom, John), k.Eq(livingroom, George));
                var clue6 = k.Noto(k.Eq(knife, dining));
                var clue7 = k.Noto(k.Eq(study, Yolanda), k.Eq(pantry, Yolanda));
                var clue8 = k.Eq(firearm, George);
                var clue9 = k.All(k.Eq(X, pantry), k.Eq(X, gas));
                return k.All(
                    k.Is(X, person),
                    k.Is(bathroom, person),
                    k.Is(dining, person),
                    k.Is(kitchen, person),
                    clue5,
                    k.Is(livingroom, person),
                    k.Is(pantry, person),
                    k.Is(study, person),
                    clue2,
                    locationConst,
                    k.Is(bag, person),
                    k.Is(firearm, person),
                    clue8,
                    k.Is(gas, person),
                    k.Is(knife, person),
                    k.Is(poison, person),
                    k.Is(rope, person),
                    weaponConst,
                    clue1,
                    clue3,
                    clue4,
                    clue6,
                    clue7,
                    clue9,
                    k.Eq(q, k.List(
                        bathroom, dining, kitchen, livingroom, pantry, study,
                        bag, firearm, gas, knife, poison, rope,
                        X)));
            });
            Console.WriteLine("(bathroom dining kitchen livingroom pantry study bag firearm gas knife poison rope X)");
            KRunner.PrintResult(res);
        }
    }

    static class CrimeLib
    {
        public static Goal Noto(this KRunner k, params Goal[] gs)
        {
            if (gs.Length == 0) return k.Succeed;
            if (gs.Length == 1) return k.If(gs[0], k.Fail, k.Succeed);
            return k.All(gs.Select(g => k.Noto(g)).ToArray());
        }

        public static Goal Distincto(this KRunner k, params object[] vs)
        {
            if (vs.Length < 2) return k.Succeed;
            if (vs.Length == 2) return k.Noto(k.Eq(vs[0], vs[1]));
            var gs = Enumerable.Empty<Goal>().ToList();
            for (var i = 0; i < vs.Length; ++i)
            {
                for (var j = i + 1; j < vs.Length; ++j)
                {
                    gs.Add(k.Noto(k.Eq(vs[i], vs[j])));
                }
            }
            return k.All(gs.ToArray());
        }

        public static Goal Is(this KRunner k, FreshVariable v, KPair set)
        {
            return k.Membero(v, set);
        }
    }
}
