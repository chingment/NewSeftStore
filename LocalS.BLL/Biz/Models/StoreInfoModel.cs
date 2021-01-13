﻿using LocalS.Entity;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class MapPoint
    {
        public double Lng { get; set; }
        public double Lat { get; set; }

        public MapPoint()
        {

        }
        public MapPoint(double lng, double lat)
        {
            this.Lng = lng;
            this.Lat = lat;
        }
    }

    public class StoreInfoModel
    {
        public StoreInfoModel()
        {
            this.AddressPoint = new MapPoint();
        }
        public string StoreId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public MapPoint AddressPoint { get; set; }
        public bool IsDelete { get; set; }
        public string MerchId { get; set; }
        public string MerchName { get; set; }
        public string[] AllMachineIds { get; set; }
        public string[] SellMachineIds { get; set; }
        public string BriefDes { get; set; }
        public bool IsOpen { get; set; }
        public List<ImgSet> DisplayImgUrls { get; set; }
        public bool IsTestMode { get; set; }
        public string SctMode { get; set; }
        public string[] GetSellChannelRefIds(E_SellChannelRefType shopMode)
        {
            string[] sellChannelRefIds = null;
            if (shopMode == E_SellChannelRefType.Machine)
            {
                sellChannelRefIds = this.SellMachineIds;
            }
            else if (shopMode == E_SellChannelRefType.Mall)
            {
                sellChannelRefIds = new string[] { SellChannelStock.MallSellChannelRefId };
            }


            return sellChannelRefIds;
        }
    }
}
