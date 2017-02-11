using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace URLEvaluator.Models
{
    public class LinkHelpers
    {
        static public string CleanUrlFromProtocol(string url)
        {
            var regex = new Regex("^[a-zA-Z]+://");
            var match = regex.Match(url);
            if (match != null)
            {
                url = url.Remove(match.Index, match.Length);
            }
            if (url.LastIndexOf("/") == url.Length - 1)
            {
                url = url.Substring(0, url.Length - 1);
            }
            if(url.IndexOf("www.") >= 0)
            {
                url = url.Remove(url.IndexOf("www."), 4);
            }
            return url;
        }

        static public string MergeRootUrlAndSubpageUrlToAbsolute(string root, string url)
        {
            if(root.Length < 2 || url.Length == 0)
            {
                return root + url;
            }
            if (url.ElementAt(0) != '/')
            {
                url = "/" + url;
            }

            if (root.ElementAt(root.Length - 1) == '/')
            {
                root = root.Substring(0, root.Length - 1);
            }
            return root + url;
        }
    }
}