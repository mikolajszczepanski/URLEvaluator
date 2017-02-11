using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace URLEvaluator.Models
{

    public enum LinkType
    {
        Absolute,
        Relative,
        Other
    }

    public class Link
    {
        public string Url { get; set; }
        public LinkType Type { get; set; }
    }
}