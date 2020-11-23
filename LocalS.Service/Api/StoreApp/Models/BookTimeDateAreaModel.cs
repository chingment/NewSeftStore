using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class BookTimeDateAreaModel
    {
        public BookTimeDateAreaModel()
        {
            this.TimeArea = new List<BookTimeTimeAreaModel>();
        }

        public string Week { get; set; }
        public string Date { get; set; }
        public string Value { get; set; }
        public int Status { get; set; }
        public string Tip { get; set; }
        public List<BookTimeTimeAreaModel> TimeArea { get; set; }
    }
}
