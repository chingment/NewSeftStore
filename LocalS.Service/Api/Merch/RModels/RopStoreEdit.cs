﻿using LocalS.BLL.Biz;
using Lumos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopStoreEdit
    {
        public RopStoreEdit()
        {
         
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string ContactAddress { get; set; }
        public string BriefDes { get; set; }
        public List<ImgSet> DisplayImgUrls { get; set; }
        public bool IsOpen { get; set; }
    }
}
