using System;
using System.Collections.Generic;
using System.Linq;

namespace NMiniKanren
{
    public class KPair
    {
        public object Lhs { get; set; }
        public object Rhs { get; set; }

        public KPair(object lhs, object rhs)
        {
            Lhs = lhs;
            Rhs = rhs;
        }

        public static KPair List(IEnumerable<object> lst)
        {
            var fst = lst.FirstOrDefault();
            if (fst == null)
            {
                return null;
            }
            return new KPair(fst, List(lst.Skip(1)));
        }

        public static bool IsList(object o)
        {
            // 空链表
            if (o == null)
            {
                return true;
            }
            // 非空链表
            if (o is KPair p)
            {
                // 递归
                return IsList(p.Rhs);
            }
            // 非链表
            return false;
        }

        public static bool Memeber(object e, object lst)
        {
            // 空链表
            if (lst == null)
            {
                return false;
            }
            // 非空链表
            if (lst is KPair p)
            {
                if (p.Lhs == null && e == null || p.Lhs.Equals(e))
                {
                    return true;
                }
                else
                {
                    // 递归
                    return Memeber(e, p.Rhs);
                }
            }
            // 非链表
            return false;
        }

        public IList<object> ToList()
        {
            if (Rhs == null) return Enumerable.Repeat(Lhs, 1).ToList();
            if (Rhs is KPair p)
            {
                var lst = p.ToList();
                lst.Insert(0, Lhs);
                return lst;
            }
            throw new InvalidOperationException($"{ToString()} is not a list");
        }

        private static string ItemToString(object item)
        {
            return item?.ToString() ?? "()";
        }

        public override string ToString()
        {
            if (IsList(this))
            {
                return $"({string.Join(" ", ToList().Select(ItemToString))})";
            } else
            {
                return $"({ItemToString(Lhs)} . {ItemToString(Rhs)})";
            }
        }

    }

}
