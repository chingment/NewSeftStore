using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreSvcChat
{
    public class RetOwnGetContactInfos
    {
        public RetOwnGetContactInfos()
        {
            this.Contacts = new List<ContactModel>();
        }

        public List<ContactModel> Contacts { get; set; }


        public class ContactModel
        {
            public string Username { get; set; }
            public string Nickname { get; set; }
            public string Avatar { get; set; }
        }
    }
}
