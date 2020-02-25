using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public enum E_CallerApp
    {
        Unknow = 0,
        Wxmp= 1
    }

    public class RupOrderPayOptions
    {
        public E_CallerApp CallerApp { get; set; }
    }
}
