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
                          Id = 2,
                          Name = "Admin",
                          NormalizedName = "ADMIN"
                      },
                      new Role
                      {
                          Id = 3,
                          Name = " User",
                          NormalizedName = "USER"
                      }
                  );

        }

    }
}
