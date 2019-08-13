using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LocalS.Entity
{
    [Table("InsCarPlateNoInfo")]
    public class InsCarPlateNoInfo
    {
        [Key]
        public string Id { get; set; }
        public string PlateNo { get; set; }
        public string Vin { get; set; }
        public string EngineNo { get; set; }
        public DateTime RegisterDate { get; set; }
        public DateTime IssueDate { get; set; }
        public string ModelCode { get; set; }
        public string ModelName { get; set; }
        public string Exhaust { get; set; }
        public string MarketYear { get; set; }
        public int Seat { get; set; }
        public decimal PurchasePrice { get; set; }
        public string Quality { get; set; }
        public string Weight { get; set; }
        public bool IsTransfer { get; set; }
        public DateTime TransferDate { get; set; }
        public bool IsCompanyCar { get; set; }
        public string OwnerName { get; set; }
        public string OwnerCertNo { get; set; }
        public string OwnerMobile { get; set; }
        public string OwnerAddress { get; set; }
        public string Creator { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
