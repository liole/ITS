namespace ITS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TestCreator : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tests", "UserID", c => c.Int(nullable: false));
            CreateIndex("dbo.Tests", "UserID");
            AddForeignKey("dbo.Tests", "UserID", "dbo.Users", "ID", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tests", "UserID", "dbo.Users");
            DropIndex("dbo.Tests", new[] { "UserID" });
            DropColumn("dbo.Tests", "UserID");
        }
    }
}
