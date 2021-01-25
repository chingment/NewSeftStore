using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetAdInitRelease
    {
        public RetAdInitRelease()
        {
            this.Belongs = new List<object>();
        }
        public E_AdSpaceId AdSpaceId { get; set; }
        public string AdSpaceName { get; set; }
        public string AdSpaceDescription { get; set; }

        public List<Object> Belongs { get; set; }
    }
}
