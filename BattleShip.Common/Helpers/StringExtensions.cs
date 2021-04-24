using System;

namespace BattleShip.Common.Helpers
{
    public static class StringExtensions
    {
        public static bool IsEqual(this string str1, string str2) =>
            str1.Equals(str2, StringComparison.OrdinalIgnoreCase);
    }
}
