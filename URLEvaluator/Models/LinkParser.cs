using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace URLEvaluator.Models
{
    public class LinkParser : ILinkParser
    {
        public IList<Link> ExtractLinksFromData(string data)
        {
            data = data.ToLower();
            var links = new List<Link>();
            var pattern = "<a[\\sa-z0-9-._~:/?#\\[\\]@!$&'\"()*+,;=]*href\\s*=\\s*['\"][a-z0-9-._~:/?#\\[\\]@!$&'()*+,;=]*['\"]";

            var regex = new Regex(pattern);

            foreach (Match matchLink in regex.Matches(data))
            {
                var link = new Link();
                link.Url = CleanLink(matchLink.Value);
                link.Type = GetTypeOfLink(link.Url);
                links.Add(link);
            }

            return links;
        }

        private string CleanLink(string url)
        {
            url = url.Substring(url.IndexOf("href"));
            var firstQuoteIndex = url.IndexOf("\"");
            if (firstQuoteIndex < 0)
            {
                firstQuoteIndex = url.IndexOf("'");
            }
            var lastQuoteIndex = url.LastIndexOf("\"");
            if (lastQuoteIndex < 0)
            {
                lastQuoteIndex = url.LastIndexOf("'");
            }
            if (firstQuoteIndex < 0 || lastQuoteIndex < 0 || firstQuoteIndex == lastQuoteIndex)
            {
                return url;
            }
            return url.Substring(firstQuoteIndex + 1, lastQuoteIndex - firstQuoteIndex - 1);
        }

        private LinkType GetTypeOfLink(string url)
        {
            Uri uri;
            if(Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                return LinkType.Absolute;
            }
            else if (Uri.TryCreate(url, UriKind.Relative, out uri))
            {
                return LinkType.Relative;
            }
            return LinkType.Other;
        }
    }
}