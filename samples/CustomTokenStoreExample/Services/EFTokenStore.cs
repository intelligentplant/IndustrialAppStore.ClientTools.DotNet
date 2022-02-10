using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using IntelligentPlant.IndustrialAppStore.Authentication;

using Microsoft.AspNetCore.Authentication;

namespace ExampleMvcApplication.Services {

    /// <summary>
    /// <see cref="ITokenStore"/> implementation that stores user tokens in an EF Core database 
    /// context.
    /// </summary>
    /// <remarks>
    ///   Access tokens and refresh tokens are encrypted at rest using the <see cref="ProtectedData"/> 
    ///   class. Note that this requires the application to be running on Windows!
    /// </remarks>
    public class EFTokenStore : TokenStore {

        private readonly UserTokensDbContext _dbContext;


        public EFTokenStore(
            IndustrialAppStoreAuthenticationOptions options, 
            HttpClient httpClient, 
            ISystemClock clock, 
            UserTokensDbContext dbContext
        ) : base(options, httpClient, clock) { 
            _dbContext = dbContext; 
        }


        protected override ValueTask InitAsync() {
            // Ensure that the entity model has been created in the SQLite database. This is only
            // required because we are not using migrations in this project!
            _dbContext.Database.EnsureCreated();
            return default;
        }


        protected override async ValueTask<OAuthTokens?> GetTokensAsync() {
            // TODO: Consider using in-memory caching instead of reading from the database on every call.

            var dbTokens = await _dbContext.Tokens.FindAsync(UserId, SessionId);
            if (dbTokens == null) {
                return null;
            }

            // Decrypt access token and refresh token.
            return new OAuthTokens(
                dbTokens.TokenType, 
                dbTokens.AccessToken == null ? null : UnprotectToken(dbTokens.AccessToken), 
                dbTokens.RefreshToken == null ? null : UnprotectToken(dbTokens.RefreshToken), 
                dbTokens.ExpiryTime
            );
        }


        protected override async ValueTask SaveTokensAsync(OAuthTokens tokens) {
            var dbTokens = await _dbContext.Tokens.FindAsync(UserId, SessionId);
            if (dbTokens == null) {
                dbTokens = new Models.UserTokens() { 
                    UserId = UserId,
                    SessionId = SessionId
                };
                _dbContext.Tokens.Add(dbTokens);
            }

            // Encrypt access token and refresh token.
            dbTokens.TokenType = tokens.TokenType;
            dbTokens.AccessToken = ProtectToken(tokens.AccessToken);
            dbTokens.RefreshToken = tokens.RefreshToken == null 
                ? null 
                : ProtectToken(tokens.RefreshToken);
            dbTokens.ExpiryTime = tokens.UtcExpiresAt;

            await _dbContext.SaveChangesAsync();
        }


        private static string UnprotectToken(string protectedToken) {
            // TODO: allow the use of entropy when unprotecting tokens.
            var protectedBytes = Convert.FromBase64String(protectedToken);
            var unprotectedBytes = ProtectedData.Unprotect(protectedBytes, Array.Empty<byte>(), DataProtectionScope.LocalMachine);
            return Encoding.UTF8.GetString(unprotectedBytes);
        }


        private static string ProtectToken(string unprotectedToken) {
            // TODO: allow the use of entropy when protecting tokens.
            var unprotectedBytes = Encoding.UTF8.GetBytes(unprotectedToken);
            var protectedBytes = ProtectedData.Protect(unprotectedBytes, Array.Empty<byte>(), DataProtectionScope.LocalMachine);
            return Convert.ToBase64String(protectedBytes);
        }

    }
}
