using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using URLEvaluator.Models;
using System.Threading.Tasks;

namespace URLEvaluator.Hubs
{
    public class MeasureSitePerformanceHub : Hub
    {

        public void EvaluateResponseTime(string url)
        {
            MeasureSitePerformanceConfig siteConfig = new MeasureSitePerformanceConfig() { Url = url };

            var sitePerformance = new MeasureSitePerformance(siteConfig, new LinkParser());
            sitePerformance.ProcessSuccessRequestEventHandler += SuccessResponse;
            sitePerformance.ProcessFailedRequestEventHandler += FailResponse;
            sitePerformance.ProcessFailedRootRequestEventHandler += RootUrlError;

            SendGroupGuid(sitePerformance.GetGroupGuid());
            sitePerformance.EvaluateResponseTimeForSiteMap();

            Clients.Caller.end();
        }

        public void LoadHistory(string url,Guid? currentGroupGuid = null)
        {
            using(var db = new HistorySitePerformanceContext())
            {
                var history = db.HistoryOfSitePerformance
                    .Where( s => s.RootSite == url && (currentGroupGuid == null || s.GroupGuid != currentGroupGuid))
                    .OrderByDescending(s => s.Date)
                    .ToList();

                Parallel.ForEach(history, h => {
                    Clients.Caller.getResultHistory(
                        h.Site,
                        h.MeasuredTime != null ? string.Format("{0:0.0000}", h.MeasuredTime.Value.TotalSeconds) : null,
                        h.GroupGuid,
                        h.Date.ToString("dd-MM-yyyy HH:mm:ss")
                    );
                });
            }
        }

        private void SendGroupGuid(Guid groupGuid)
        {
            //TODO
        }

        private void RootUrlError()
        {
            Clients.Caller.error("Given url is incorrect or unaccessable.");
        }

        private void FailResponse(string url)
        {
            Clients.Caller.urlFailResponse(url);
        }

        private void SuccessResponse(string url, TimeSpan? time)
        {
            Clients.Caller.urlSuccessResponse(url, string.Format("{0:0.0000}",time.Value.TotalSeconds));
        }
    }
}