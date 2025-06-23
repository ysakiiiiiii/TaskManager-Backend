using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Models.Domain;

namespace TaskManagerBackend.Data
{
    public class StoreDbContext : IdentityDbContext<User>
    {
        public StoreDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var adminRoleId = "09f53d7c-72bd-4582-a57f-ac35e3c61f2f";
            var userRoleId = "e4fb4f0c-224b-46bb-b479-e22718f6a8fe";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id=adminRoleId,
                    ConcurrencyStamp=adminRoleId,
                    Name="Admin",
                    NormalizedName="Admin".ToUpper()
                },
                new IdentityRole
                {
                    Id=userRoleId,
                    ConcurrencyStamp=userRoleId,
                    Name="User",
                    NormalizedName= "User".ToUpper()
                }

            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
 