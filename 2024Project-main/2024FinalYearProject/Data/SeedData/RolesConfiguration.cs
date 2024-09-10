using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace _2024FinalYearProject.Data.SeedData
{
    public class RolesConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Name = "User",
                    NormalizedName = "USER"
                }
                , new IdentityRole
                {
                    Name = "Consultant",
                    NormalizedName = "CONSULTANT"
                },
                new IdentityRole
                {
                    Name = "FinAdvisor",
                    NormalizedName = "FINADVISOR"
                }
            );
        }
    }
}
