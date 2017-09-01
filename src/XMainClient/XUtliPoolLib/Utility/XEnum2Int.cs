using System;
using System.Collections.Generic;

namespace XUtliPoolLib
{
    public struct XEnum2Int<TEnum> : IEqualityComparer<TEnum>
        where TEnum : struct
    {
        public static int ToInt(TEnum en)
        {
            return EnumInt32ToInt.Convert(en);
        }

        public bool Equals(TEnum lhs, TEnum rhs)
        {
            return ToInt(lhs) == ToInt(rhs);
        }

        public int GetHashCode(TEnum en)
        {
            return ToInt(en);
        }
    }
}
