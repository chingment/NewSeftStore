using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.UI
{
    public class FsTemplateData
    {
        public string Type { get; set; }

        public object Value { get; set; }


        public class TmplOrderSku
        {
            public string UniqueId { get; set; }
            public E_UniqueType UniqueType { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
            public string MainImgUrl { get; set; }
            public string Quantity { get; set; }
            public string ChargeAmount { get; set; }
            public bool IsGift { get; set; }
            public string StatusName { get; set; }
            public FsText Desc { get; set; }
        }
    }
}
