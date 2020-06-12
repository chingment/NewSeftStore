using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RupProductSearch
    {
        public string StoreId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string KindId { get; set; }
        public string SubjectId { get; set; }
        public E_SellChannelRefType ShopMode { get; set; }
        public string Name { get; set; }
    }
}
