﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class MachineInfoModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LogoImgUrl { get; set; }
        public string MerchId { get; set; }
        public string StoreId { get; set; }
        public string CsrQrCode { get; set; }

        public string MerchName { get; set; }

        public string StoreName { get; set; }

    }
}