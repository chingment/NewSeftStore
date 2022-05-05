using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class SvDataJd
    {
        public bool IsHidValue { get; set; }
        public string Id { get; set; }
        public string Name { get; set; }
        //具体值
        public object Value { get; set; }
        public string ValueText { get; set; }
        //提示颜色
        public string Color { get; set; }
        //提示文字
        public string Tips { get; set; }
        //提示符号
        public string Sign { get; set; }
        //参考范围
        public string RefRange { get; set; }

        public string Pph { get; set; }
        public List<object> RefRanges { get; set; }

        public object Chat { get; set; }
        public SvDataJd()
        {
            this.RefRanges = new List<object>();
        }

        public void Set(string tips, string sign, string color)
        {
            this.Tips = tips;
            this.Sign = sign;
            this.Color = color;
        }

        public class RefRangeArea
        {
            public object Min { get; set; }

            public object Max { get; set; }

            public string Color { get; set; }

            public string Tips { get; set; }
        }
    }
}
