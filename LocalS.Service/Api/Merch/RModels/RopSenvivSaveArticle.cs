using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopSenvivSaveArticle
    {
        public string Id { get; set; }

        public string Title { get; set; }
        public List<string> Tags { get; set; }

        public string Content { get; set; }
    }
}
