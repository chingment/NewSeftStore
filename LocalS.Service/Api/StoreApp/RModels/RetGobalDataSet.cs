using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RetGobalDataSet
    {
        public IndexPageModel Index { get; set; }

        public ProductKindPageModel ProductKind { get; set; }

        public CartPageModel Cart { get;set;}

        public PersonalPageModel Personal { get; set; }
    }
}
