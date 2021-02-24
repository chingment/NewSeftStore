using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopMemberRightSetFeeSt
    {
        public string FeeStId { get; set; }
        public decimal OriginalValue { get; set; }
        public decimal SaleValue { get; set; }
        public bool IsStop { get; set; }
    }
}
