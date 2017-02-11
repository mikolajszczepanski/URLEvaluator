using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace URLEvaluator.Models
{
    public class MeasureSitePerformanceConfig
    {
        [Url(ErrorMessage = "Url is not correct.")]
        public string Url { get; set; }
    }
}