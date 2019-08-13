using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.InsApp
{
    public class InsCarCompanyRuleModel
    {
        public string CompanyId { get; set; }

        public string CompanyName { get; set; }

        public string CompanyImgUrl { get; set; }
       
        public int CommissionRate { get; set; }
    }
}
