namespace URLEvaluator.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Start : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HistorySitePerformances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RootSite = c.String(),
                        Site = c.String(),
                        MeasuredTime = c.Time(precision: 7),
                        Date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.HistorySitePerformances");
        }
    }
}
