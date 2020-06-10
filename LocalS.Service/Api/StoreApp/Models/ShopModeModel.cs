using LocalS.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.StoreApp
{
    public class ShopModeModel
    {
        public E_ShopMode Id { get; set; }

        public string Name { get; set; }

        public bool Selected { get; set; }
    }
}
