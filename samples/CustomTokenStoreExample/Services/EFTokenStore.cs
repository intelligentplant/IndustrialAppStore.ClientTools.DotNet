using IntelligentPlant.IndustrialAppStore.Authentication;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

namespace ExampleMvcApplication.Services {

    /// <summary>
    /// <see cref="ITokenStore"/> implementation that stores user tokens in an EF Core database 
    /// context.
    /// </summary>
    /// <remarks>
    ///   Access tokens and refresh tokens are encrypted at rest using ASP.NET Core data protection.
    /// </remarks>
    public class EFTokenStore : TokenStore {

        private readonly UserTokensDbContext _dbContext;

        private readonly IDataProtector _dataProtector;


        public EFTokenStore(
            IOptions<IndustrialAppStoreAuthenticationOptions> options, 
            HttpClient httpClient, 
            TimeProvider timeProvider, 
            UserTokensDbContext dbContext,
            IDataProtectionProvider dataProtectionProvider
        ) : base(options, httpClient, timeProvider) { 
            _dbContext = dbContext;
            _dataProtector = dataProtectionProvider.CreateProtector(typeof(EFTokenStore).FullName!, "tokens", "v1");
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
                UnprotectToken(dbTokens.AccessToken)!, 
                UnprotectToken(dbTokens.RefreshToken), 
                dbTokens.ExpiryTime
            );
        }


        protected override async ValueTask SaveTokensAsync(OAuthTokens tokens) {
            var dbTokens = await _dbContext.Tokens.FindAsync(UserId, SessionId);
            if (dbTokens == null) {
                dbTokens = new Models.UserTokens() { 
                    UserId = UserId!,
                    SessionId = SessionId!
                };
                _dbContext.Tokens.Add(dbTokens);
            }

            // Encrypt access token and refresh token.
            dbTokens.TokenType = tokens.TokenType;
            dbTokens.AccessToken = ProtectToken(tokens.AccessToken)!;
            dbTokens.RefreshToken = ProtectToken(tokens.RefreshToken);
            dbTokens.ExpiryTime = tokens.UtcExpiresAt;

            await _dbContext.SaveChangesAsync();
        }


        private string? UnprotectToken(string? protectedToken) {
            if (protectedToken == null) {
                return null;
            }

            return _dataProtector.Unprotect(protectedToken);
        }


        private string? ProtectToken(string? unprotectedToken) {
            if (unprotectedToken == null) {
                return null;
            }

            return _dataProtector.Protect(unprotectedToken);
        }

    }
}
