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
    
    public class ProjectsController : Controller
    {
        public ActionResult Index()
        {
            string gig = WebConfigurationManager.AppSettings["CurrentGIG"];
            ViewData["gig"] = gig;

            List<Models.Video> videos;
            using (StreamReader sr = new StreamReader(Server.MapPath($"~/Content/{gig}/info.json")))
            {
                videos = JsonConvert.DeserializeObject<List<Models.Video>>(sr.ReadToEnd());
            }
            return View(videos);
        }
    }
}