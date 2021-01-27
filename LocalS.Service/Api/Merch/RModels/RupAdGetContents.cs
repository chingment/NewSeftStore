using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RupAdGetContents : RupBaseGetList
    {
        public E_AdSpaceId AdSpaceId { get;set;}
    }
}
