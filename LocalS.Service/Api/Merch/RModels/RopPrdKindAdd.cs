using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopPrdKindAdd
    {
        public string PId { get; set; }
        public string Name { get; set; }
        public string IconImgUrl { get; set; }
        public List<ImgSet> DispalyImgUrls { get; set; }
        public string Description { get; set; }
    }
}
