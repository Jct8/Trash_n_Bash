using System;
using System.Collections.Generic;

namespace SheetCodesEditor
{
    public class TypeSorter : IComparer<Type>
    {
        public int Compare(Type x, Type y)
        {
            return string.Compare(x.Name, y.Name);
        }
    }
}
