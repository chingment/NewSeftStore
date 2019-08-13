using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("InsCarModelInfo")]
    public class InsCarModelInfo
    {
        [Key]
        public string Id { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string ModelCode { get; set; }
        public string ModelName { get; set; }
        public string Alias { get; set; }
        public string CarType { get; set; }
        public string Vin { get; set; }
        public string MarketYear { get; set; }
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
        public int Seat { get; set; }
        public string Quality { get; set; }
        public string Weight { get; set; }
        public string Exhaust { get; set; }
        public string BelongCode { get; set; }
        public string BelongName { get; set; }
        public decimal PurchasePrice { get; set; }
        public string EnergySourceType { get; set; }
        public string EnergySourceName { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
