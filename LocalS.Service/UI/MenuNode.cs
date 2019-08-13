using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.UI
{
    public class MenuNode
    {
        public MenuNode()
        {
            // this.Children = new List<MenuNode>();
        }
        public string Id { get; set; }
        public string PId { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Icon { get; set; }
        public string Path { get; set; }
        public bool IsSidebar { get; set; }
        public bool IsNavbar { get; set; }
        public string Component { get; set; }

        public bool IsRouter { get; set; }
    }
}
