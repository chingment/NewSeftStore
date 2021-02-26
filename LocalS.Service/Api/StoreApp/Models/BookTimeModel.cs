using LocalS.BLL.Biz;
using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{

    public enum E_TabMode
    {

        Unknow = 0,
        Delivery = 1,
        SelfTakeByStore = 2,
        DeliveryOrSelfTakeByStore = 3,
        SelfTakeByMachine = 4,
        FeeByMember = 5,
        FeeByRent = 6
    }

    public enum TipType
    {
        Unknow = 0,
        NoCanUse = 1,
        CanUse = 2,
        InUse = 3
    }

    public class BookTimeModel
    {
        public string Text { get; set; }
        public string Week { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
    }

}
