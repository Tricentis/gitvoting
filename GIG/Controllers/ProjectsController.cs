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

namespace GIG.Controllers {

    public class ProjectsController : Controller {

        private GIGContext db = new GIGContext();
        private string currentgigyear = WebConfigurationManager.AppSettings["CurrentGIG"];

        public ActionResult Index() {
            var votes = db.Votes
                   .Where(v => v.Year == currentgigyear)
                   .GroupBy(v => v.Team)
                    .Select(g => new { video = g.Key, count = g.Count() }).ToDictionary(t => t.video, t=> t.count);

            var votedByMe = db.Votes
                    .Where(v => v.Year == currentgigyear && v.Username == User.Identity.Name)
                    .Select(v => new { v.Team })
                    .ToDictionary(d => d.Team, d => d.Team);

            ViewData["gig"] = currentgigyear;

            List<Video> videos;
            using (StreamReader sr = new StreamReader(Server.MapPath($"~/Content/{currentgigyear}/info.json"))) {
                videos = JsonConvert.DeserializeObject<List<Video>>(sr.ReadToEnd());
            }
            foreach (Video video in videos) {
                if (votes.ContainsKey(video.Team)) {
                    video.Votes = votes[video.Team];
                }

                video.VotedByMe = votedByMe.ContainsKey(video.Team);
            }

            return View(videos);
        }

        [HttpPost]
        public ActionResult Vote(string videoId) {
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