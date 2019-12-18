﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.UI
{
    public class FsBlock
    {
        public FsBlock()
        {
            this.Tag = new FsTag();
            this.Data = new List<FsTemplateData>();
            this.Qrcode = new FsQrcode();
        }

        public FsTag Tag { get; set; }
        public List<FsTemplateData> Data { get; set; }
        public FsQrcode Qrcode { get; set; }
    }
}
