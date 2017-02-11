using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace URLEvaluator.Models
{
    public class HistorySitePerformance
    {
        [Key]
        public int Id { get; set; }
        public string RootSite { get; set; }
        public string Site { get; set; }
        public TimeSpan? MeasuredTime { get; set; }
        public DateTime Date { get; set; }
        public Guid GroupGuid { get; set; }
    }

    public class HistorySitePerformanceContext : DbContext
    {
        public HistorySitePerformanceContext(): base("DefaultConnection")
        {

        }

        public virtual DbSet<HistorySitePerformance> HistoryOfSitePerformance { get; set; }
    }
}