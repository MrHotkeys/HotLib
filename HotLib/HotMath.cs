namespace HotLib
{
    public static class HotMath
    {
        /// <summary>
        /// Gets the quotient and remainder from dividing two numbers, implemented using only one actual division operation.
        /// </summary>
        /// <param name="dividend">The number being divided.</param>
        /// <param name="divisor">The number to divide by.</param>
        /// <param name="quotient">The result of dividing the dividend by the divisor.</param>
        /// <param name="remainder">The remainder, or result of applying modulo to the dividend and divisor.</param>
        public static void DivRem(int dividend, int divisor, out int quotient, out int remainder)
        {
            quotient = dividend / divisor;
            remainder = dividend - (quotient * divisor);
        }

        /// <summary>
        /// Gets the quotient and remainder from dividing two numbers, implemented using only one actual division operation.
        /// </summary>
        /// <param name="dividend">The number being divided.</param>
        /// <param name="divisor">The number to divide by.</param>
        /// <param name="quotient">The result of dividing the dividend by the divisor.</param>
        /// <param name="remainder">The remainder, or result of applying modulo to the dividend and divisor.</param>
        public static void DivRem(long dividend, long divisor, out long quotient, out long remainder)
        {
            quotient = dividend / divisor;
            remainder = dividend - (quotient * divisor);
        }

        /// <summary>
        /// Gets the quotient and remainder from dividing by 2, implemented using bitwise operations.
        /// </summary>
        /// <param name="dividend">The number being divided.</param>
        /// <param name="quotient">The result of dividing the dividend by the divisor.</param>
        /// <param name="remainder">The remainder, or result of applying modulo to the dividend and divisor.</param>
        public static void DivRem2(uint dividend, out uint quotient, out byte remainder)
        {
            quotient = dividend >> 1;
            remainder = (byte)(dividend - (quotient * 2));
        }

        /// <summary>
        /// Gets the quotient and remainder from dividing by 4, implemented using bitwise operations.
        /// </summary>
        /// <param name="dividend">The number being divided.</param>
        /// <param name="quotient">The result of dividing the dividend by the divisor.</param>
        /// <param name="remainder">The remainder, or result of applying modulo to the dividend and divisor.</param>
        public static void DivRem4(uint dividend, out uint quotient, out byte remainder)
        {
            quotient = dividend >> 2;
            remainder = (byte)(dividend - (quotient * 4));
        }

        /// <summary>
        /// Gets the quotient and remainder from dividing by 8, implemented using bitwise operations.
        /// </summary>
        /// <param name="dividend">The number being divided.</param>
        /// <param name="quotient">The result of dividing the dividend by the divisor.</param>
        /// <param name="remainder">The remainder, or result of applying modulo to the dividend and divisor.</param>
        public static void DivRem8(uint dividend, out uint quotient, out byte remainder)
        {
            quotient = dividend >> 3;
            remainder = (byte)(dividend - (quotient * 8));
        }

        /// <summary>
        /// Gets the quotient and remainder from dividing by 8, implemented using bitwise operations.
        /// </summary>
        /// <param name="dividend">The number being divided.</param>
        /// <param name="quotient">The result of dividing the dividend by the divisor.</param>
        /// <param name="remainder">The remainder, or result of applying modulo to the dividend and divisor.</param>
        public static void DivRem8(ulong dividend, out ulong quotient, out byte remainder)
        {
            quotient = dividend >> 3;
            remainder = (byte)(dividend - (quotient * 8));
        }

        /// <summary>
        /// Gets the quotient and remainder from dividing by 16, implemented using bitwise operations.
        /// </summary>
        /// <param name="dividend">The number being divided.</param>
        /// <param name="quotient">The result of dividing the dividend by the divisor.</param>
        /// <param name="remainder">The remainder, or result of applying modulo to the dividend and divisor.</param>
        public static void DivRem16(uint dividend, out uint quotient, out byte remainder)
        {
            quotient = dividend >> 4;
            remainder = (byte)(dividend - (quotient * 16));
        }

        /// <summary>
        /// Gets the quotient and remainder from dividing by 32, implemented using bitwise operations.
        /// </summary>
        /// <param name="dividend">The number being divided.</param>
        /// <param name="quotient">The result of dividing the dividend by the divisor.</param>
        /// <param name="remainder">The remainder, or result of applying modulo to the dividend and divisor.</param>
        public static void DivRem32(uint dividend, out uint quotient, out byte remainder)
        {
            quotient = dividend >> 5;
            remainder = (byte)(dividend - (quotient * 32));
        }

        /// <summary>
        /// Gets the quotient and remainder from dividing by 64, implemented using bitwise operations.
        /// </summary>
        /// <param name="dividend">The number being divided.</param>
        /// <param name="quotient">The result of dividing the dividend by the divisor.</param>
        /// <param name="remainder">The remainder, or result of applying modulo to the dividend and divisor.</param>
        public static void DivRem64(uint dividend, out uint quotient, out byte remainder)
        {
            quotient = dividend >> 6;
            remainder = (byte)(dividend - (quotient * 64));
        }
    }
}
