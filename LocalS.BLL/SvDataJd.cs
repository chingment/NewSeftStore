using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class SvDataJd
    {
        //具体值
        public string Value { get; set; }
        //提示颜色
        public string Color { get; set; }
        //提示文字
        public string Tips { get; set; }
        //提示符号
        public string Sign { get; set; }
        //参考范围
        public string RefRange { get; set; }

        public SvDataJd()
        {

        }

        public void Set(string tips, string sign,string color)
        {
            this.Tips = tips;
            this.Sign = sign;
            this.Color = color;
        }
    }
}
