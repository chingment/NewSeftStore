using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.BLL
{
    public class SvDataJd
    {
        public string Color { get; set; }
        public string Value { get; set; }
        public string Tips { get; set; }
        public string Sign { get; set; }
        public SvDataJd()
        {

        }

        public SvDataJd(string value, string tips, string sign, string color)
        {
            this.Value = value;
            this.Tips = tips;
            this.Sign = sign;
            this.Color = color;
        }
    }
}
