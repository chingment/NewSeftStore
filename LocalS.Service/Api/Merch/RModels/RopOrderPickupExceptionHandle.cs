using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopOrderPickupExceptionHandle
    {
        public string UniqueId { get; set; }

        public ExceptionHandleMethod HandleMethod { get; set; }

        public string Remark { get; set; }


        public enum ExceptionHandleMethod
        {
            Unknow = 0,
            /// <summary>
            /// 机器已开门，但没有发现
            /// </summary>
            MachineHaveOpenDoorButNoFindGoods = 1,
            GoodsHaveBeenDeliveredToCustomer = 2,
            GoodsNotHaveBeenDeliveredToCustomer = 3
        }
    }
}
