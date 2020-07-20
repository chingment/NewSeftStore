using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Admin
{
    public class RetPrdKindInitEdit
    {

        public string PId { get; set; }
        public string PName { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        public List<ImgSet> DisplayImgUrls { get; set; }
        public string Description { get; set; }
    }
}
