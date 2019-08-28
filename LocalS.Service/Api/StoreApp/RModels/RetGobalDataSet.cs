using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RetGobalDataSet
    {
        public RetIndexPageData Index { get; set; }

        public RetProductKindPageData ProductKind { get; set; }

        public RetCartPageData Cart { get;set;}

        public RetPersonalPageData Personal { get; set; }
    }
}
