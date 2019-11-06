using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class StatusModel
    {
        public StatusModel()
        {

        }

        public StatusModel(int value, string text)
        {
            this.Value = value;
            this.Text = text;
        }

        public int Value { get; set; }
        public string Text { get; set; }
    }
}
