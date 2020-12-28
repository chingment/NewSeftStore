using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{

    public enum E_Coupon_UseMode
    {
        Unknow = 0,
        Pay = 1,
        ScanCode = 2
    }
    public enum E_Coupon_UseAreaType
    {

        Unknow = 0,
        //全部通用
        All = 1,
        //指定店铺
        Store = 2,
        //指定商品分类
        ProductKind = 3,
        //指定商品
        ProductSpu = 4
    }
    public enum E_Coupon_FaceType
    {

        Unknow = 0,
        //购物代金券
        ShopVoucher = 1,
        //购物折扣券
        ShopDiscount = 2,
        //租金代金券
        RentVoucher = 3,
        //押金代金券
        DepositVoucher = 4,
        //入场券
        EntranceTicket = 5
    }
    public enum E_Coupon_Category
    {

        Unknow = 0,
        All = 1,
        NewUser = 2,
        Memeber = 3,
        Shopping = 4
    }
    public enum E_Coupon_ShopMode
    {

        Unknow = 0,
        Mall = 1,
        Machine = 3
    }
    public enum E_Coupon_UseTimeType
    {

        Unknow = 0,
        //有效日
        ValidDay = 1,
        //时间段
        TimeArea = 2
    }
    public enum E_Coupon_PerLimitTimeType
    {

        Unknow = 0,
        //无限制
        UnLimit = 1,
        //日
        Day = 2,
        //月
        Month = 3,
        //季
        Quarter = 4,
    }

    [Table("Coupon")]
    public class Coupon
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string Name { get; set; }
        public E_Coupon_Category Category { get; set; }
        public E_Coupon_ShopMode ShopMode { get; set; }
        public int IssueQuantity { get; set; }
        public int UsedQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public E_Coupon_FaceType FaceType { get; set; }
        public decimal FaceValue { get; set; }
        public int PerLimitNum { get; set; }
        public E_Coupon_PerLimitTimeType PerLimitTimeType { get; set; }
        public int PerLimitTimeNum { get; set; }
        public string LimitMemberLevels { get; set; }
        public decimal AtLeastAmount { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public E_Coupon_UseAreaType UseAreaType { get; set; }
        public E_Coupon_UseMode UseMode { get; set; }
        public string UseAreaValue { get; set; }
        public E_Coupon_UseTimeType UseTimeType { get; set; }
        public string UseTimeValue { get; set; }
        public string Description { get; set; }
        public bool IsDelete { get; set; }
        public bool IsSuperposition { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
