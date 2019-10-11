﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAlipaySdk
{
    public class AlipayAppInfoConfig
    {
        public string AppId { get; set; }
        public string AppPrivateKey { get; set; }
        public string AlipayPublicKey { get; set; }

        public string PayResultNotifyUrl { get; set; }
    }
}
