using GIG.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace GIG.Controllers {

    [Authorize]
    public class LeaderboardController : Controller {

        private GIGContext db = new GIGContext();
        private string currentgigyear = WebConfigurationManager.AppSettings["CurrentGIG"];

        public ActionResult Index() {
            var votes = db.Votes
                   .Where(v => v.Year == currentgigyear)
                   .GroupBy(v => v.Team)
                    .Select(g => new { video = g.Key, count = g.Count() })
                    .OrderByDescending(g => g.count)
                    .ToDictionary(g => g.video, g => g.count); ;

            return View(votes);
        }
    }
}