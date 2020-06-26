using System;
using System.Collections.Generic;
using System.Linq;

namespace NMiniKanren
{
    public class KRunner
    {
        private int _freshCounter = 0;

        private KRunner() { }

        public static IList<object> Run(int? n, Func<KRunner, FreshVariable, Goal> body)
        {
            var k = new KRunner();
            // 定义待求解的未知量q
            var q = k.Fresh();
            // 执行body，得到最终目标g
            var g = body(k, q);
            // 初始上下文是一个空替换，应用到g，得到包含可行替换的Stream s
            var s = g(Substitution.Empty());
            // 从s中取出前n个（n==null则取所有）替换，查找各个替换下q的解，并给自由变量换个好看的符号。
            return s.MapAndTake(n, sub => Renumber(sub.Walk(q)));
        }

        public FreshVariable Fresh()
        {
            return new FreshVariable(_freshCounter++);
        }

        public KPair Pair(object v1, object v2)
        {
            return new KPair(v1, v2);
        }

        public KPair List(params object[] vs)
        {
            return KPair.List(vs);
        }

        public Goal Succeed = sub => Stream.Unit(sub);
        public Goal Fail => sub => Stream.Empty();

        public Goal Eq(object v1, object v2)
        {
            return sub =>
            {
                var u = sub.Unify(v1, v2);
                if (u == null)
                {
                    return Stream.Empty();
                }
                return Stream.Unit(u);
            };
        }

        #region All

        public Goal And(Goal g1, Goal g2)
        {
            return sub => g1(sub).Bind(g2);
        }

        public Goal All(params Goal[] gs)
        {
            if (gs.Length == 0) return Succeed;
            if (gs.Length == 1) return gs[0];
            return And(gs[0], All(gs.Skip(1).ToArray()));
        }

        public Goal Andi(Goal g1, Goal g2)
        {
            return sub => g1(sub).Bindi(g2);
        }

        public Goal Alli(params Goal[] gs)
        {
            if (gs.Length == 0) return Succeed;
            if (gs.Length == 1) return gs[0];
            return Andi(gs[0], All(gs.Skip(1).ToArray()));
        }

        #endregion

        #region Any

        public Goal Or(Goal g1, Goal g2)
        {
            return sub => g1(sub).Append(() => g2(sub));
        }

        public Goal Any(params Goal[] gs)
        {
            if (gs.Length == 0) return Fail;
            if (gs.Length == 1) return gs[0];
            return Or(gs[0], Any(gs.Skip(1).ToArray()));
        }

        public Goal Ori(Goal g1, Goal g2)
        {
            return sub => g1(sub).Interleave(() => g2(sub));
        }

        public Goal Anyi(params Goal[] gs)
        {
            if (gs.Length == 0) return Fail;
            if (gs.Length == 1) return gs[0];
            return Ori(gs[0], Anyi(gs.Skip(1).ToArray()));
        }

        #endregion

        #region If
        public Goal If(Goal g1, Goal g2, Goal g3)
        {
            return sub =>
            {
                var s = g1(sub);
                if (s.IsEmpty()) return g3(sub);
                return s.Bind(g2);
            };
        }
        #endregion

        public Goal Recurse(Func<Goal> f)
        {
            return sub => f()(sub);
        }

        private static object RenumberRec(object x, IList<string> vars)
        {
            if (x is FreshVariable v)
            {
                var varName = v.ToString();
                var i = vars.IndexOf(varName);
                if (i < 0)
                {
                    i = vars.Count;
                    vars.Add(varName);
                }
                return new FreshVariable(i, true);
            } else if (x is KPair p)
            {
                var lhs = RenumberRec(p.Lhs, vars);
                var rhs = RenumberRec(p.Rhs, vars);
                return new KPair(lhs, rhs);
            } else
            {
                return x;
            }
        }

        private static object Renumber(object x)
        {
            return RenumberRec(x, Enumerable.Empty<string>().ToList());
        }

        public static void PrintResult(IList<object> res)
        {
            Console.WriteLine($"[{string.Join(", ", res.Select(o => o.ToString()))}]");
        }

    }
}
