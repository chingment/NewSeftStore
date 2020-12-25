﻿using LocalS.BLL;
using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class ProductSkuDetailsModel
    {
        private bool _isOffSell = true;


        public string Id { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string MainImgUrl { get; set; }
        public string BriefDes { get; set; }
        public bool IsShowPrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal ShowPrice { get; set; }

        public int SellQuantity { get; set; }

        public bool IsOffSell
        {
            get
            {
                return _isOffSell;
            }
            set
            {
                _isOffSell = value;
            }
        }
        public List<string> CharTags { get; set; }
        public List<SpecItem> SpecItems { get; set; }
        public string SpecIdx { get; set; }
        public List<ImgSet> DisplayImgUrls { get; set; }
        public List<ImgSet> DetailsDes { get; set; }
        public List<SpecIdxSku> SpecIdxSkus { get; set; }
        public bool IsUseRent { get; set; }
        public decimal RentAmount { get; set; }
        public E_RentTermUnit RentTermUnit { get; set; }
        public string RentTermUnitText { get; set; }
        public decimal DepositAmount { get; set; }
    }
}
