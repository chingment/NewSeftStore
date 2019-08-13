using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YsyInscarSdk;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Api api = new Api();

            var api1 = new QueryCarInfoApi("粤A8K96A");

            var get1 = api.Get<QueryCarInfoResult>(api1);

           
        }
    }
}
