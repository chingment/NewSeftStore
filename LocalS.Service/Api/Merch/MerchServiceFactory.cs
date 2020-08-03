﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalS.Service.Api.Merch
{
    public class MerchServiceFactory
    {
        public static HomeService Home
        {
            get
            {
                return new HomeService();
            }
        }

        public static AdminUserService AdminUser
        {
            get
            {
                return new AdminUserService();
            }
        }

        public static ClientUserService ClientUser
        {
            get
            {
                return new ClientUserService();
            }
        }

        public static PrdProductService PrdProduct
        {
            get
            {
                return new PrdProductService();
            }
        }

        public static StoreService Store
        {
            get
            {
                return new StoreService();
            }
        }

        public static MachineService Machine
        {
            get
            {
                return new MachineService();
            }
        }

        public static OrderService Order
        {
            get
            {
                return new OrderService();
            }
        }

        public static PayTransService PayTrans
        {
            get
            {
                return new PayTransService();
            }
        }

        public static AdSpaceService AdSpace
        {
            get
            {
                return new AdSpaceService();
            }
        }

        public static ReportService Report
        {
            get
            {
                return new ReportService();
            }
        }

        public static LogService Log
        {
            get
            {
                return new LogService();
            }
        }
    }
}
