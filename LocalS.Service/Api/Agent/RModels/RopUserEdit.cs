﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Agent
{
    public class RopUserEdit
    {
        public string Id{ get; set; }
        public string Password { get; set; }

        public string PhoneNumber { get; set; }

        public string FullName { get; set; }

        public string Email { get; set; }

        public bool IsDisable { get; set; }
    }
}
