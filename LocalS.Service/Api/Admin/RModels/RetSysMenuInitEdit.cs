﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Admin
{
    public class RetSysMenuInitEdit
    {
        public string PId { get; set; }
        public string PTitle { get; set; }
        public string PName { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Icon { get; set; }
        public string Description { get; set; }
        public bool IsSidebar { get; set; }
        public bool IsNavbar { get; set; }
        public bool IsRouter { get; set; }
    }
}
