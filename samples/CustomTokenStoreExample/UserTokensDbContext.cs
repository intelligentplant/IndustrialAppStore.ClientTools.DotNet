using System;

using ExampleMvcApplication.Models;

using Microsoft.EntityFrameworkCore;

namespace ExampleMvcApplication {

    /// <summary>
    /// Example Entity Framework Core context for storing user access tokens.
    /// </summary>
    public class UserTokensDbContext : DbContext {

        /// <summary>
        /// User access tokens.
        /// </summary>
        public DbSet<UserTokens> Tokens { get; set; }


        /// <summary>
        /// Creates a new <see cref="UserTokensDbContext"/> object.
        /// </summary>
        public UserTokensDbContext() : base() { }


        /// <summary>
        /// Creates a new <see cref="UserTokensDbContext"/> object using the specified 
        /// <see cref="DbContextOptions"/>.
        /// </summary>
        /// <param name="options">
        ///   The options.
        /// </param>
        public UserTokensDbContext(DbContextOptions<UserTokensDbContext> options) : base(options) { }


        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            // Configure composite primary key.
            modelBuilder.Entity<UserTokens>().HasKey(x => new { x.UserId, x.SessionId });
        }

    }
}
