﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GIG.Controllers
{
    [Authorize]
    public class ArchiveController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}