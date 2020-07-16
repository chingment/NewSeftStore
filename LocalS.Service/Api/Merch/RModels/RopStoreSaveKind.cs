using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopStoreSaveKind
    {
        public string StoreId { get; set; }

        public string Id { get; set; }
        public string Name { get; set; }
        public List<ImgSet> DisplayImgUrls { get; set; }
        public string Description { get; set; }
        public bool IsSynElseStore { get; set; }
    }
}
