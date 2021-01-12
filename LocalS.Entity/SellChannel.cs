using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LocalS.Entity
{
    public enum E_UniqueType
    {
        Unknow = 0,
        Order = 1,
        OrderSub = 2
    }

    public enum E_SellChannelRefType
    {
        Unknow = 0,
        Mall = 1,//商城库存
        Shop = 2,//门店库存
        Machine = 3,//机器库存
    }

    public enum E_ReceiveMode
    {
        Unknow = 0,
        Delivery = 1,
        SelfTakeByStore = 2,
        SelfTakeByMachine = 4,
        FeeByMember = 5,
        ConsumeByStore = 6,
        FeeByDeposit = 7,
        FeeByRent = 8
    }

}
