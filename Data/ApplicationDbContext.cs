using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spider_EMT.Data.Account;

namespace Spider_EMT.Data
{
    public class ApplicationDbContext: IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply common properties to Identity tables
            ConfigureAuditColumns(builder.Entity<IdentityUserClaim<int>>());
            ConfigureAuditColumns(builder.Entity<IdentityUserLogin<int>>());
            ConfigureAuditColumns(builder.Entity<IdentityUserToken<int>>());
            ConfigureAuditColumns(builder.Entity<IdentityRole<int>>());
            ConfigureAuditColumns(builder.Entity<IdentityRoleClaim<int>>());
            ConfigureAuditColumns(builder.Entity<IdentityUserRole<int>>());
        }

        private void ConfigureAuditColumns<TEntity>(EntityTypeBuilder<TEntity> entity) where TEntity : class
        {
            entity.Property<DateTime?>("CreateDate").HasColumnType("datetime2");
            entity.Property<int?>("CreateUserId").IsRequired(false);
            entity.Property<DateTime?>("UpdateDate").HasColumnType("datetime2");
            entity.Property<int?>("UpdateUserId").IsRequired(false);
        }
    }
}
 