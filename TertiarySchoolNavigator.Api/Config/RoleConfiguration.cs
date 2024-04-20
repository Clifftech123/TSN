using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TertiarySchoolNavigator.Api.Models;

namespace TertiarySchoolNavigator.Api.Config
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.HasData(


                      new Role
                      {
                          Id = Guid.NewGuid(),
                          Name = "Admin",
                          NormalizedName = "ADMIN"
                      },
                      new Role
                      {
                          Id = Guid.NewGuid(),
                          Name = " User",
                          NormalizedName = "USER"
                      }
                  );

        }

    }
}
