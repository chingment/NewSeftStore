﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Account
{
    public class RopOwnLoginByMinProgram
    {
        public string MerchId { get; set; }
        public string AppId { get; set; }
        public string Code { get; set; }
        public string Iv { get; set; }
        public string EncryptedData { get; set; }
    }
}
