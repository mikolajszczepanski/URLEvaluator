using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace URLEvaluator.Models
{

    class SitePerformance
    {
        public string Url { get; set; }
        public TimeSpan? Time { get; set; }
    }

    public class MeasureSitePerformance
    {
        private readonly MeasureSitePerformanceConfig siteConfig;
        private IList<SitePerformance> visitedSites = new List<SitePerformance>();
        private ILinkParser linkParser;
        private readonly ushort attempts;
        private readonly ushort retryAttempts;
        private object lockObject = new object();
        private readonly Guid groupGuid = Guid.NewGuid();

        public event Action<string,TimeSpan?> ProcessSuccessRequestEventHandler;
        public event Action<string> ProcessFailedRequestEventHandler;
        public event Action ProcessFailedRootRequestEventHandler;

        public MeasureSitePerformance(
            MeasureSitePerformanceConfig siteConfig, 
            ILinkParser linkParser, 
            ushort attempts = 3,
            ushort retryAttempts = 3)
        {
            this.siteConfig = siteConfig;
            this.linkParser = linkParser;
            if(attempts == 0 || retryAttempts == 0)
            {
                throw new ArgumentException("Attemps cannot be less than 1");
            }
            this.attempts = attempts;
            this.retryAttempts = retryAttempts;
        }

        public void EvaluateResponseTimeForSiteMap()
        {
            string rootUrl = siteConfig.Url.ToLower();
            string rootPage = LinkHelpers.CleanUrlFromProtocol(rootUrl);

            EvaluateResponseTimeForRootPage(rootPage, rootUrl);

            SaveVisitedSitesToDatabase();
        }

        private void SaveVisitedSitesToDatabase()
        {
            using (var db = new HistorySitePerformanceContext())
            {
                foreach (var visitedSite in visitedSites)
                {
                    if (visitedSite != null)
                    {
                        var performance = new HistorySitePerformance()
                        {
                            MeasuredTime = visitedSite.Time,
                            RootSite = siteConfig.Url,
                            Site = visitedSite.Url,
                            Date = DateTime.Now,
                            GroupGuid = groupGuid
                        };
                        db.HistoryOfSitePerformance.Add(performance);
                    }
                }
                db.SaveChanges();
            }
        }

        public Guid GetGroupGuid()
        {
            return groupGuid;
        }

        private bool FilterLink(Link link, string rootPage)
        {
            if(link.Type == LinkType.Relative)
            {
                var url = LinkHelpers.MergeRootUrlAndSubpageUrlToAbsolute(rootPage, link.Url);
                return visitedSites.ToList().Find(v => v.Url == url) == null;
            }
            else if (link.Type == LinkType.Absolute)
            {
                return visitedSites.ToList().Find(v => v.Url == link.Url) == null;
            }
            return false;
        }

        private void EvaluateResponseTimeForRootPage(string rootPage, string rootUrl)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    //var watch = Stopwatch.StartNew();
                    string siteData = webClient.DownloadString(rootUrl);
                    //watch.Stop();
                    var links = linkParser.ExtractLinksFromData(siteData);
                    links = links.ToList().Where(l => FilterLink(l, rootPage)).Distinct().ToList();
                    //var performance = new SitePerformance() { Time = watch.Elapsed, Url = rootUrl };
                    //visitedSites.Add(performance);
                    //ProcessSuccessRequestEventHandler(performance.Url, performance.Time);
                    Parallel.ForEach(links.ToList(), l =>
                    {
                        var newRequestUrl = l.Type == LinkType.Absolute ? l.Url : LinkHelpers.MergeRootUrlAndSubpageUrlToAbsolute(rootUrl, l.Url);
                        for (int i = 0; i < attempts; i++)
                        {
                            EvaluateResponseTimeForSubPage(rootPage, newRequestUrl, retryAttempts);
                        }
                    });
                }
            }
            catch(WebException ex)
            {
                ProcessFailedRootRequestEventHandler();
            }
        }

        private void EvaluateResponseTimeForSubPage(string rootPage, string requestedUrl,ushort retryAttempts)
        {
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    var watch = Stopwatch.StartNew();
                    string siteData = webClient.DownloadString(requestedUrl);
                    watch.Stop();
                    var performance = new SitePerformance() { Time = watch.Elapsed, Url = requestedUrl };
                    lock (lockObject)
                    {
                        visitedSites.Add(performance);
                    }
                    ProcessSuccessRequestEventHandler(performance.Url, performance.Time);
                }
            }
            catch (WebException ex)
            {
                var performance = new SitePerformance() { Time = null, Url = requestedUrl };
                visitedSites.Add(performance);
                ProcessFailedRequestEventHandler(requestedUrl);
                if (retryAttempts > 0)
                {
                    retryAttempts--;
                    EvaluateResponseTimeForSubPage(rootPage, requestedUrl, retryAttempts);
                }
            }
        }



    }
}