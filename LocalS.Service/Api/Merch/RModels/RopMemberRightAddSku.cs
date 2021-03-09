using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopMemberRightAddSku
    {
        public string[] StoreIds { get; set; }
        public string SkuId { get; set; }
        public string LevelStId { get; set; }
        public decimal MemberPrice { get; set; }
        public string[] ValidDate { get; set; }
        public bool IsDisabled { get; set; }
    }
}
