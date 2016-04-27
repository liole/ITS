namespace ITS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserGroup : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GroupTests",
                c => new
                    {
                        Group_ID = c.Int(nullable: false),
                        Test_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Group_ID, t.Test_ID })
                .ForeignKey("dbo.Groups", t => t.Group_ID, cascadeDelete: true)
                .ForeignKey("dbo.Tests", t => t.Test_ID, cascadeDelete: true)
                .Index(t => t.Group_ID)
                .Index(t => t.Test_ID);
            
            CreateTable(
                "dbo.UserGroups",
                c => new
                    {
                        User_ID = c.Int(nullable: false),
                        Group_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_ID, t.Group_ID })
                .ForeignKey("dbo.Users", t => t.User_ID, cascadeDelete: true)
                .ForeignKey("dbo.Groups", t => t.Group_ID, cascadeDelete: true)
                .Index(t => t.User_ID)
                .Index(t => t.Group_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserGroups", "Group_ID", "dbo.Groups");
            DropForeignKey("dbo.UserGroups", "User_ID", "dbo.Users");
            DropForeignKey("dbo.GroupTests", "Test_ID", "dbo.Tests");
            DropForeignKey("dbo.GroupTests", "Group_ID", "dbo.Groups");
            DropIndex("dbo.UserGroups", new[] { "Group_ID" });
            DropIndex("dbo.UserGroups", new[] { "User_ID" });
            DropIndex("dbo.GroupTests", new[] { "Test_ID" });
            DropIndex("dbo.GroupTests", new[] { "Group_ID" });
            DropTable("dbo.UserGroups");
            DropTable("dbo.GroupTests");
        }
    }
}
