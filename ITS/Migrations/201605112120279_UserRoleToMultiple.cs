namespace ITS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserRoleToMultiple : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "IsStudent", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "IsTeacher", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "IsAdmin", c => c.Boolean(nullable: false));
            DropColumn("dbo.Users", "Role");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "Role", c => c.Int(nullable: false));
            DropColumn("dbo.Users", "IsAdmin");
            DropColumn("dbo.Users", "IsTeacher");
            DropColumn("dbo.Users", "IsStudent");
        }
    }
}
