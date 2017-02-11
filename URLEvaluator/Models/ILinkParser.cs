using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace URLEvaluator.Models
{
    public interface ILinkParser
    {
        IList<Link> ExtractLinksFromData(string data);
    }
}
