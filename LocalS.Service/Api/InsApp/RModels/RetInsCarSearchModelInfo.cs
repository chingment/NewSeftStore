using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.InsApp
{
    public class RetInsCarSearchModelInfo
    {
        public RetInsCarSearchModelInfo()
        {
            this.Models = new List<InsCarModelInfoModel>();
        }

        public List<InsCarModelInfoModel> Models { get; set; }
    }
}
