using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YsyInscarSdk
{
    public class BaseResult<T>
    {
        public string result_code { get; set; }
        public string result_desc { get; set; }
        public string result_id { get; set; }
        public T data { get; set; }
    }
}
