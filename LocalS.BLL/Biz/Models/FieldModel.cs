﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL.Biz
{
    public class FieldModel
    {
        public FieldModel()
        {

        }

        public FieldModel(object value, string text)
        {
            this.Value = value;
            this.Text = text;
        }

        public object Value { get; set; }
        public string Text { get; set; }
    }
}
