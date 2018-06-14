using GIG.Data;
using GIG.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GIG.Controllers {

    //[Authorize]
    public class ProjectsController : Controller {

        private GIGContext db = new GIGContext();
        private string currentgigyear = WebConfigurationManager.AppSettings["CurrentGIG"];
        private bool votingEnabled = Boolean.Parse(WebConfigurationManager.AppSettings["VotingEnabled"]);

        public ActionResult Index(string sortOrder) {
            var votes = db.Votes
                   .Where(v => v.Year == currentgigyear)
                   .GroupBy(v => v.Team)
                    .Select(g => new { video = g.Key, count = g.Count() }).ToDictionary(t => t.video, t=> t.count);

            var votedByMe = db.Votes
                    .Where(v => v.Year == currentgigyear && v.Username == User.Identity.Name)
                    .Select(v => new { v.Team })
                    .ToDictionary(d => d.Team, d => d.Team);

            ViewData["gig"] = currentgigyear;
            ViewData["sortorder"] = string.IsNullOrEmpty(Request["sortOrder"]) ? "team" : Request["sortOrder"];

            List<Video> videos = new List<Video>();
            try {
                using (StreamReader sr = new StreamReader(Server.MapPath($"~/Content/{currentgigyear}/info.json"))) {
                    videos = JsonConvert.DeserializeObject<List<Video>>(sr.ReadToEnd());
                }
            } catch {
                // no videos
            }

            if (videos == null) { return View();  }

            foreach (Video video in videos) {
                if (votes.ContainsKey(video.Team)) {
                    video.Votes = votes[video.Team];
                }

                video.VotedByMe = votedByMe.ContainsKey(video.Team);
            }

            switch (sortOrder) {
                case "team":
                    videos = videos.OrderBy(v => v.Team).ToList();
                    break;
                case "votes":
                    videos = videos.OrderByDescending(v => v.Votes).ToList();
                    break;
                case "shuffle":
                    videos = videos.OrderBy(v => Guid.NewGuid()).ToList();
                    break;
                default:
                    videos = videos.OrderBy(v => v.Team).ToList();
                    break;
            }

            return View(videos);
        }

        [HttpPost]
        public ActionResult Vote(string videoId) {
            if (!votingEnabled) {
                Response.StatusCode = (int)HttpStatusCode.Forbidden;
                return Json("Voting Not Enabled Yet");
            }

            var myVote = db.Votes.Where(
                            v => v.Year == currentgigyear && 
                            v.Team == videoId && 
                            v.Username == User.Identity.Name)
                         .SingleOrDefault();

            var voted = false;

            if (myVote == null) {
                var vote = db.Votes.Create();
                vote.Team = videoId;
                vote.Username = User.Identity.Name;
                vote.Year = currentgigyear;
                db.Votes.Add(vote);
                db.SaveChanges();
                voted = true;
            } else {
                db.Votes.Remove(myVote);
                db.SaveChanges();
                voted = false;
            }

            var votes = db.Votes.Where(v => v.Year == currentgigyear && v.Team == videoId).Count();

            var result = new { votes = votes, voted = voted, video = videoId };
            return Json(result);
        }
    }
}