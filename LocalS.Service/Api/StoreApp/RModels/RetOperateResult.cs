﻿using LocalS.BLL.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class RetOperateResult
    {
        public RetOperateResult()
        {
            this.Fields = new List<FsField>();
            this.Buttons = new List<FsButton>();
        }
        public ResultType Result { get; set; }
        public string Message { get; set; }
        public string Remarks { get; set; }
        public bool IsComplete { get; set; }
        public List<FsField> Fields { get; set; }
        public List<FsButton> Buttons { get; set; }

        public bool IsShowContactButton { get; set; }
        public enum ResultType
        {
            Unknown = 0,
            Success = 1,
            Failure = 2,
            Exception = 3,
            Tips = 4
        }
    }
}
