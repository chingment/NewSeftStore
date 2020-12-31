﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class RetClientUserInitManageBaseInfo
    {
        public string Id { get; set; }

        public string Avatar { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string FullName { get; set; }

        public string NickName { get; set; }

        public bool IsHasProm { get; set; }
        public bool IsStaff { get; set; }
    }
}
