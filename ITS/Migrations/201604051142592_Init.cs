namespace ITS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Questions",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        QuestionText = c.String(nullable: false),
                        Coefficient = c.Decimal(nullable: false, precision: 18, scale: 2),
                        TestID = c.Int(nullable: false),
                        AnswerA = c.String(),
                        AnswerB = c.String(),
                        AnswerC = c.String(),
                        AnswerD = c.String(),
                        Answer = c.Int(),
                        Answer1 = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Tests", t => t.TestID, cascadeDelete: true)
                .Index(t => t.TestID);
            
            CreateTable(
                "dbo.Tests",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        SubjectID = c.Int(nullable: false),
                        Randomize = c.Boolean(nullable: false),
                        Mark = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Subjects", t => t.SubjectID, cascadeDelete: true)
                .Index(t => t.SubjectID);
            
            CreateTable(
                "dbo.Subjects",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Results",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TestID = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                        Mark = c.Int(nullable: false),
                        QuestionAmountCorrect = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Tests", t => t.TestID, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserID, cascadeDelete: true)
                .Index(t => t.TestID)
                .Index(t => t.UserID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false),
                        LastName = c.String(nullable: false),
                        Login = c.String(nullable: false),
                        Password = c.String(nullable: false),
                        Role = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Results", "UserID", "dbo.Users");
            DropForeignKey("dbo.Results", "TestID", "dbo.Tests");
            DropForeignKey("dbo.Questions", "TestID", "dbo.Tests");
            DropForeignKey("dbo.Tests", "SubjectID", "dbo.Subjects");
            DropIndex("dbo.Results", new[] { "UserID" });
            DropIndex("dbo.Results", new[] { "TestID" });
            DropIndex("dbo.Tests", new[] { "SubjectID" });
            DropIndex("dbo.Questions", new[] { "TestID" });
            DropTable("dbo.Users");
            DropTable("dbo.Results");
            DropTable("dbo.Groups");
            DropTable("dbo.Subjects");
            DropTable("dbo.Tests");
            DropTable("dbo.Questions");
        }
    }
}
