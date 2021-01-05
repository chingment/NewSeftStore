using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class SelfTakeModel
    {
        public SelfTakeModel()
        {
            this.BookTime = new BookTimeModel();
            this.Contact = new ContactModel();
            this.Mark = new MarkModel();
        }

        public BookTimeModel BookTime { get; set; }
        public ContactModel Contact { get; set; }
        public MarkModel Mark { get; set; }
    }

    public class MarkModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string AreaName { get; set; }
        public string AreaCode { get; set; }
        public string Consignee { get; set; }
        public string PhoneNumber { get; set; }
    }
}
