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

        public ActionResult Index() {
            string gig = WebConfigurationManager.AppSettings["CurrentGIG"];
            ViewData["gig"] = gig;

            List<GIG.Models.Video> videos;
            using (StreamReader sr = new StreamReader(Server.MapPath($"~/Content/{gig}/info.json"))) {
                videos = JsonConvert.DeserializeObject<List<GIG.Models.Video>>(sr.ReadToEnd());
            }
            return View(videos);
        }

        [HttpPost]
        public ActionResult Vote(string videoId) {
            var result = new { votes = 16, voted = true, video = videoId };
            return Json(result);
        }
    }
}