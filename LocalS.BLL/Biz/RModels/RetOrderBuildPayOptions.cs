using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class RetOrderBuildPayOptions
    {
        public RetOrderBuildPayOptions()
        {
            this.Options = new List<Option>();
        }

        public string Title { get; set; }
        public List<Option> Options { get; set; }
        public class Option
        {
            public E_OrderPayCaller PayCaller { get; set; }
            public E_OrderPayPartner PayPartner { get; set; }
            public List<E_OrderPayWay> PaySupportWays { get; set; }
            public string Title { get; set; }
            public string Desc { get; set; }
            public bool IsSelect { get; set; }
        }
    }
}
