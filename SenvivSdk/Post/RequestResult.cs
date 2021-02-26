using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SenvivSdk
{
    public class RequestResult<T>
    {
        public int Code { get; set; }

        public T Data { get; set; }
    }
}
