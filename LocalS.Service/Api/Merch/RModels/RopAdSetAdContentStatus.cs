using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RopAdSpaceSetAdContentStatus
    {
        public string Id { get; set; }

        public E_AdContentStatus Status { get; set; }
    }
}
