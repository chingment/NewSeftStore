﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreTerm
{
    public class RopDeviceInitData
    {
        public string ImeiId { get; set; }
        public string DeviceId { get; set; }
        public string MacAddress { get; set; }
        public string AppVersionCode { get; set; }
        public string AppVersionName { get; set; }
        public string CtrlSdkVersionCode { get; set; }
    }
}