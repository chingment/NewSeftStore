using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RetGobalDataSet
    {
        public RetIndexGetPageData Index { get; set; }

        public RetProductKindGetPageData ProductKind { get; set; }

        public RetCartGetPageData Cart { get;set;}

        public RetPersonalGetPageData Personal { get; set; }
    }
}
