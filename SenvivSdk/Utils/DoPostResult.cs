using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SenvivSdk
{
    public class DoPostResult
    {
        public HttpStatusCode StatusCode { get; set; }

        public string ResponseString { get; set; }
    }
}
