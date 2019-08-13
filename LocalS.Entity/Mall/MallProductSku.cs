using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LocalS.Entity
{
    [Table("MallProductSku")]
    public class MallProductSku
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string KindIds { get; set; }
        public string SubjectIds { get; set; }
        public string RecipientModeIds { get; set; }
        public string Name { get; set; }
        public string BarCode { get; set; }
        public string SimpleCode { get; set; }
        public string ImgUrl { get; set; }
        public string DispalyImgUrls { get; set; }
        public decimal ShowPrice { get; set; }
        public decimal SalePrice { get; set; }
        public string DetailsDes { get; set; }
        public string SpecDes { get; set; }
        public string BriefDes { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? MendTime { get; set; }
        public string Mender { get; set; }
    }
}
