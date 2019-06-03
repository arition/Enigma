using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using EnigmaLib;
using EnigmaLib.Model;
using EnigmaServer.Database;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace EnigmaServer
{
    public class BearerAuthenticationHandler : AuthenticationHandler<BearerAuthenticationOptions>
    {
        private readonly DatabaseContext _database;

        public BearerAuthenticationHandler(IOptionsMonitor<BearerAuthenticationOptions> options, ILoggerFactory logger,
            UrlEncoder encoder, ISystemClock clock, DatabaseContext databaseContext) : base(options, logger, encoder,
            clock)
        {
            _database = databaseContext;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization")) return AuthenticateResult.NoResult();

            if (!AuthenticationHeaderValue.TryParse(Request.Headers["Authorization"], out var headerValue))
                return AuthenticateResult.NoResult();

            if (!"Bearer".Equals(headerValue.Scheme, StringComparison.OrdinalIgnoreCase))
                return AuthenticateResult.NoResult();

            SignedData signedData;
            try
            {
                var headerValueBytes = Convert.FromBase64String(headerValue.Parameter);
                var json = Encoding.UTF8.GetString(headerValueBytes);
                signedData = JsonConvert.DeserializeObject<SignedData>(json);
            }
            catch
            {
                return AuthenticateResult.NoResult();
            }

            using (var hash = SHA256.Create())
            {
                if (!signedData.SHA256Hash.IsEqual(hash.ComputeHash(signedData.Content)))
                    return AuthenticateResult.Fail("Invalid Hash");
            }

            using (var rsa = RSA.Create(signedData.PublicKey))
            {
                if (!rsa.VerifyHash(signedData.SHA256Hash, signedData.Signature, HashAlgorithmName.SHA256,
                    RSASignaturePadding.Pkcs1))
                    return AuthenticateResult.Fail("Invalid Signature");
            }

            if (int.TryParse(Encoding.UTF8.GetString(signedData.Content), out var unixSecond))
            {
                if (!(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 60 < unixSecond &&
                      unixSecond < DateTimeOffset.UtcNow.ToUnixTimeSeconds() + 60))
                    return AuthenticateResult.Fail("Invalid Content");
            }
            else
            {
                return AuthenticateResult.Fail("Invalid Content");
            }

            var publicKeyJson = JsonConvert.SerializeObject(signedData.PublicKey);
            var user = await _database.User.Where(t => t.PublicKeyString == publicKeyJson).FirstOrDefaultAsync();
            if (user == null)
                return AuthenticateResult.Fail("Invalid PublicKey");

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
            };
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return AuthenticateResult.Success(ticket);
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = "Bearer EnigmaPublicKey";
            return base.HandleChallengeAsync(properties);
        }
    }
}