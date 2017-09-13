namespace Repository.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CategoryInfo", "name", c => c.String(maxLength: 256));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CategoryInfo", "name", c => c.String(maxLength: 32));
        }
    }
}
