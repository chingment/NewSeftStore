﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace LocalS.Entity
{
    [Table("PrdSku")]
    public class PrdSku
    {
        [Key]
        public string Id { get; set; }
        public string MerchId { get; set; }
        public string BarCode { get; set; }
        public string CumCode { get; set; }
        public string PinYinIndex { get; set; }
        public string SpuId { get; set; }
        public string Name { get; set; }
        public decimal SalePrice { get; set; }
        public string SpecDes { get; set; }
        public string SpecIdx { get; set; }
        public string Creator { get; set; }
        public bool IsDelete { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? MendTime { get; set; }
        public string Mender { get; set; }
    }
}
