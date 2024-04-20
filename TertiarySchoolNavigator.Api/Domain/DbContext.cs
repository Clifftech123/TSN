using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TertiarySchoolNavigator.Api.Config;
using TertiarySchoolNavigator.Api.Models;
using TertiarySchoolNavigator.Api.Models.AuthModels;

namespace TertiarySchoolNavigator.Api.Domain
{
    public class AppDbContext : IdentityDbContext<User, Role, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
      : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new RoleConfiguration());
        }

        public DbSet<User> Users { get; set; }

    }
}
