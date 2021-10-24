using System;

namespace LittleJohnWebAPI.Utils
{
    internal static class RandomExtensions
    {
        private static int NextInt32(this Random rng)
        {
            var firstBits = rng.Next(0, 1 << 4) << 28;
            var lastBits = rng.Next(0, 1 << 28);
            return firstBits | lastBits;
        }

        public static decimal NextDecimal(this Random rng)
        {
            var scale = (byte)rng.Next(29);
            var sign = rng.Next(2) == 1;
            var newDecimal = new decimal(rng.NextInt32(),
                rng.NextInt32(),
                rng.NextInt32(),
                sign,
                scale);

            if (newDecimal < 0)
            {
                return newDecimal * -1;
            }

            return newDecimal;
        }
    }
}