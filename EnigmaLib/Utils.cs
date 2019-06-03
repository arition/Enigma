using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;

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

        public static RSAParameters ToPublicKey(this RSAParameters privateKey)
        {
            return new RSAParameters {Exponent = privateKey.Exponent, Modulus = privateKey.Modulus};
        }

        public static async Task<T> ReadAsAsync<T>(this HttpContent content)
        {
            var json = await content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}