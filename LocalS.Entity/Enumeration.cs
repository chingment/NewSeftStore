using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Entity
{

    public enum E_StoreStatus
    {

        Unknow = 0,
        Valid = 1,
        Invalid = 2
    }

    public enum E_StoreSellChannelRefType
    {
        Unknow = 0,
        Express = 1,
        Machine = 2
    }

    public enum E_CartStatus
    {

        Unknow = 0,
        WaitSettle = 1,
        Settling = 2,
        SettleCompleted = 3,
        Deleted = 4
    }

    public enum E_ReceptionMode
    {

        Unknow = 0,
        Express = 1,
        Machine = 2
    }

    public enum E_CartOperateType
    {

        Unknow = 0,
        Selected = 1,
        Increase = 2,
        Decrease = 3,
        Delete = 4
    }

    public enum E_CouponStatus
    {

        Unknow = 0,
        WaitUse = 1,
        Used = 2,
        Expired = 3,
        Delete = 4,
        Frozen = 5
    }

    public enum E_CouponSourceType
    {

        Unknow = 0,
        Receive = 1,
        Give = 2
    }

    public enum E_CouponType
    {

        Unknow = 0,
        FullCut = 1,
        UnLimitedCut = 2,
        Discount = 3
    }

    public enum E_AdSpaceId
    {
        Unknow = 0,
        MachineHome = 1,
        AppHomeTop = 2
    }

    public enum E_AdReleaseStatus
    {
        Unknow = 0,
        Normal = 1,
        Deleted = 2
    }

    public enum E_AdSpaceBelong
    {

        Unknow = 0,
        Machine = 1,
        MinProgram = 2,
    }

    public enum E_AdSpaceType
    {
        Unknow = 0,
        Images = 1,
        Video = 2,
    }

    public enum E_AdSpaceStatus
    {
        Unknow = 0,
        Enabled = 1,
        Disenabled = 2
    }

    public enum E_AppCaller
    {

        Unknow = 0,
        MinProgram = 1
    }

    public enum E_OrderStatus
    {

        Unknow = 0,
        Submitted = 1000,
        WaitPay = 2000,
        Payed = 3000,
        Completed = 4000,
        Cancled = 5000

    }
}
