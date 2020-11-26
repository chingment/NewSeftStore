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
        Mall = 1,//商城
        Machine = 3,//机器

    }

    public enum E_ReceiveMode
    {
        Unknow = 0,
        Delivery = 1,
        StoreSelfTake = 2,
        MachineSelfTake = 3
    }

}
