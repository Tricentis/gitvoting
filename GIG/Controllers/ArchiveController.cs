using GIG.Data;
using GIG.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GIG.Controllers
{
    [Authorize]
    public class ArchiveController : Controller
    {
        private GIGContext db = new GIGContext();
        private List<string> archivedgigs = WebConfigurationManager.AppSettings["ArchivedGIGs"].Split(',').ToList();

        public ActionResult Index(string year)
        {
            ViewData["archives"] = archivedgigs;

            if (string.IsNullOrEmpty(year)) {
                if (archivedgigs.Count > 0) {
                    year = archivedgigs.Last();
                }
            }

            if (!archivedgigs.Contains(year)) {
                return View();
            }

            ViewData["gig"] = year;
            
            var votes = db.Votes
                   .Where(v => v.Year == year)
                   .GroupBy(v => v.Team)
                    .Select(g => new { video = g.Key, count = g.Count() }).ToDictionary(t => t.video, t => t.count);

            var votedByMe = db.Votes
                    .Where(v => v.Year == year && v.Username == User.Identity.Name)
                    .Select(v => new { v.Team })
                    .ToDictionary(d => d.Team, d => d.Team);

            List<Video> videos = new List<Video>();
            try {
                using (StreamReader sr = new StreamReader(Server.MapPath($"~/Content/{year}/info.json"))) {
                    videos = JsonConvert.DeserializeObject<List<Video>>(sr.ReadToEnd());
                }
            } catch {
                // no videos
            }

            foreach (Video video in videos) {
                if (votes.ContainsKey(video.Team)) {
                    video.Votes = votes[video.Team];
                }

                video.VotedByMe = votedByMe.ContainsKey(video.Team);
            }

            return View(videos);
        }
    }
}