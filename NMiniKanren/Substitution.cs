using System;
using System.Collections.Generic;
using System.Text;

namespace NMiniKanren
{
    public class Substitution
    {
        private readonly Substitution parent;
        public FreshVariable Var { get; }
        public object Val { get; }

        private Substitution(Substitution p, FreshVariable var, object val)
        {
            parent = p;
            Var = var;
            Val = val;
        }

        private static readonly Substitution theEmptySubstitute = new Substitution(null, null, null);

        public static Substitution Empty()
        {
            return theEmptySubstitute;
        }

        public bool IsEmpty()
        {
            return this == theEmptySubstitute;
        }

        public Substitution Extend(FreshVariable var, object val)
        {
            return new Substitution(this, var, val);
        }

        public bool Find(FreshVariable var, out object val)
        {
            if (IsEmpty())
            {
                val = null;
                return false;
            }
            if (Var == var)
            {
                val = Val;
                return true;
            }
            return parent.Find(var, out val);
        }

        public object Walk(object v)
        {
            if (v is KPair p)
            {
                return new KPair(Walk(p.Lhs), Walk(p.Rhs));
            }
            if (v is FreshVariable var && Find(var, out var val))
            {
                return Walk(val);
            }
            return v;
        }

        public Substitution Unify(object v1, object v2)
        {
            v1 = Walk(v1);
            v2 = Walk(v2);
            if (v1 is KPair p1 && v2 is KPair p2)
            {
                return Unify(p1.Lhs, p2.Lhs)?.Unify(p1.Rhs, p2.Rhs);
            }
            if (v1 is FreshVariable var1)
            {
                return Extend(var1, v2);
            }
            if (v2 is FreshVariable var2)
            {
                return Extend(var2, v1);
            }
            if (v1 == null)
            {
                if (v2 == null) return this;
            } else
            {
                if (v1.Equals(v2)) return this;
            }
            return null;
        }

    }
}
