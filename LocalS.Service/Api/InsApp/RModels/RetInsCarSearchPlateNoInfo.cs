using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.InsApp
{
    public class RetInsCarSearchCarPlateNoInfo
    {
        public RetInsCarSearchCarPlateNoInfo()
        {
            this.CarInfo = new InsCarInfoModel();
            this.CarOwner = new InsCarCustomerModel();
        }

        public InsCarInfoModel CarInfo { get; set; }
        public InsCarCustomerModel CarOwner { get; set; }
    }
}
