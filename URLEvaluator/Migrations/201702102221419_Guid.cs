namespace URLEvaluator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Guid : DbMigration
    {
        public override void Up()
        {
            var guid = System.Guid.NewGuid();
            AddColumn("dbo.HistorySitePerformances", "GroupGuid", c => c.Guid(nullable: true));
            Sql(string.Format("UPDATE dbo.HistorySitePerformances SET GroupGuid = '{0}'", guid));
            AlterColumn("dbo.HistorySitePerformances", "GroupGuid", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.HistorySitePerformances", "GroupGuid");
        }
    }
}
