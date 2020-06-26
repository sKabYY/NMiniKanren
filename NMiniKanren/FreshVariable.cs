using System;
using System.Collections.Generic;
using System.Text;

namespace NMiniKanren
{
    public class FreshVariable
    { 
        private readonly int _index;
        private readonly bool _free;
        public FreshVariable(int index, bool free = false)
        {
            _index = index;
            _free = free;
        }

        public override string ToString()
        {
            return $"{(_free ? "_" : "v")}{_index}";
        }

    }
}
