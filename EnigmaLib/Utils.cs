using System.Linq;
using System.Security.Cryptography;

namespace EnigmaLib
{
    public static class Utils
    {
        public static bool IsEqual(this RSAParameters a, RSAParameters b)
        {
            if (!a.D.IsEqual(b.D))
                return false;
            if (!a.DP.IsEqual(b.DP))
                return false;
            if (!a.DQ.IsEqual(b.DQ))
                return false;
            if (!a.Exponent.IsEqual(b.Exponent))
                return false;
            if (!a.InverseQ.IsEqual(b.InverseQ))
                return false;
            if (!a.Modulus.IsEqual(b.Modulus))
                return false;
            if (!a.P.IsEqual(b.P))
                return false;
            if (!a.Q.IsEqual(b.Q))
                return false;
            return true;
        }

        public static bool IsEqual(this byte[] a, byte[] b)
        {
            if (a == null && b == null) return true;
            if (a?.Length != b?.Length)
                return false;

            return !a.Where((t, i) => t != b[i]).Any();
        }
    }
}