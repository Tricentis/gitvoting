﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace GIG.Models
{
    public class Video
    {
        public string Team { get; set; }
        public string ThumbnailFile { get; set; }
        public string AbstractFile { get; set; }
        public string Videolink { get; set; }
        public string Channelid { get; set; }
        public string Videoid { get; set; }


        public int Votes { get; set; }

        public bool VotedByMe { get; set; }
    }
}