using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.InsApp
{
    public class RetInsCarGetIndexPageData
    {

        public RetInsCarGetIndexPageData()
        {
            this.CompanyRules = new List<InsCarCompanyRuleModel>();
            this.Orders = new List<InsCarOrderModel>();
            this.SearchPlateNoRecords = new List<InsCarSearchPlateNoRecordModel>();
        }

        public List<InsCarCompanyRuleModel> CompanyRules { get; set; }

        public List<InsCarOrderModel> Orders { get; set; }

        public List<InsCarSearchPlateNoRecordModel> SearchPlateNoRecords { get; set; }


    }
}
