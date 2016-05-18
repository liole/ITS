namespace ITS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NumberQuestion : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Questions", name: "Answer1", newName: "Answer2");
            AddColumn("dbo.Questions", "Answer1", c => c.Decimal(precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Questions", "Answer1");
            RenameColumn(table: "dbo.Questions", name: "Answer2", newName: "Answer1");
        }
    }
}
