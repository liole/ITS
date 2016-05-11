namespace ITS.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
	using ITS.Domain;
	using System.Web.Helpers;
    using ITS.Domain.Entities;

    internal sealed class Configuration : DbMigrationsConfiguration<ITS.Domain.EFDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ITS.Domain.EFDbContext context)
        {
			context.Users.AddOrUpdate(
				new User()
				{
					FirstName = "School",
					LastName = "Admin",
					Login = "admin",
					Password = Crypto.HashPassword("admin"),
					IsAdmin = true
				});
        }
    }
}
