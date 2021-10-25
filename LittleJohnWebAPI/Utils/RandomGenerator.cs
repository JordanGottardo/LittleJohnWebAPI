using System;

namespace LittleJohnWebAPI.Utils
{
    internal static class RandomExtensions
    {
        public static decimal NextDecimal(this Random rng)
        {
            return Convert.ToDecimal((rng.NextDouble() * (10000 - 10) + 10));
        }
    }
}