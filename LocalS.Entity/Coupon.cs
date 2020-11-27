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
        //代金券
        Voucher = 1,
        //折扣券
        Discount = 2
    }

    public enum E_Coupon_Category
    {

        Unknow = 0,
        All = 1,
        Register = 2,
        Memeber = 3,
        Shopping = 4
    }

    public enum E_Coupon_ShopMode
    {

        Unknow = 0,
        Mall = 1,
        Machine = 3
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
        public int UseQuantity { get; set; }
        public E_Coupon_FaceType FaceType { get; set; }
        public decimal FaceValue { get; set; }
        public int PerLimitNum { get; set; }
        public decimal AtLeastAmount { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public E_Coupon_UseAreaType UseAreaType { get; set; }
        public E_Coupon_UseMode UseMode { get; set; }
        public string UseAreaValue { get; set; }
        public string Description { get; set; }
        public bool IsDelete { get; set; }
        public bool IsSuperposition { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public string Mender { get; set; }
        public DateTime? MendTime { get; set; }
    }
}
