namespace ITS.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
	using ITS.Models;
	using System.Web.Helpers;

    internal sealed class Configuration : DbMigrationsConfiguration<ITS.Models.EFDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ITS.Models.EFDbContext context)
        {
			context.Users.AddOrUpdate(
				new User()
				{
					FirstName = "School",
					LastName = "Admin",
					Login = "admin",
					Password = Crypto.HashPassword("admin"),
					Role = UserRole.Admin
				});
        }
    }
}
