using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Account
{
    public class RetHomeGetIndexPageData
    {
        public RetHomeGetIndexPageData()
        {
            this.Appcaltions = new List<_Appcaltion>();
        }

        public List<_Appcaltion> Appcaltions { get; set; }


        public class _Appcaltion
        {
            public string Name { get; set; }

            public string Url { get; set; }

            public string ImgUrl { get; set; }

            public string Describe { get; set; }
        }
    }
}
