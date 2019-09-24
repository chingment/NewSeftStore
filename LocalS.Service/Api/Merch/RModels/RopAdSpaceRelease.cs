using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopAdSpaceRelease
    {
        public E_AdSpaceId AdSpaceId { get; set; }
        public string Title { get; set; }
        public List<ImgSet> DispalyImgUrls { get; set; }
        public List<string> BelongIds { get; set; }
    }

}
