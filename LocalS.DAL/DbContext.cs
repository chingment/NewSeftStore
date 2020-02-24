﻿using LocalS.Entity;
using Lumos.DbRelay;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace LocalS.DAL
{
    public class DbContext : AuthorizeRelayDbContext
    {
        public DbContext()
            : base("DefaultConnection")
        {
            // this.Configuration.ProxyCreationEnabled = false;
        }
        public IDbSet<InsCarPlateNoSearchHis> InsCarPlateNoSearchHis { get; set; }
        public IDbSet<InsCarCompanyRule> InsCarCompanyRule { get; set; }
        public IDbSet<InsCarPlateNoInfo> InsCarPlateNoInfo { get; set; }
        public IDbSet<InsCarModelInfo> InsCarModelInfo { get; set; }
        public IDbSet<Agent> Agent { get; set; }
        public IDbSet<Merch>  Merch { get; set; }
        public IDbSet<MerchMachine> MerchMachine { get; set; }
        public IDbSet<PrdKind> PrdKind { get; set; }
        public IDbSet<PrdSubject> PrdSubject { get; set; }
        public IDbSet<PrdProduct> PrdProduct { get; set; }
        public IDbSet<PrdProductSku> PrdProductSku { get; set; }
        public IDbSet<PrdProductKind> PrdProductKind { get; set; }
        public IDbSet<PrdProductSubject> PrdProductSubject { get; set; }
        public IDbSet<Store> Store { get; set; }
        public IDbSet<SellChannelStock> SellChannelStock { get; set; }
        public IDbSet<SellChannelStockLog> SellChannelStockLog { get; set; }
        public IDbSet<ClientCart> ClientCart { get; set; }
        public IDbSet<ClientCoupon> ClientCoupon { get; set; }
        public IDbSet<AdContent> AdContent { get; set; }
        public IDbSet<AdContentBelong> AdContentBelong { get; set; }
        public IDbSet<AdSpace> AdSpace { get; set; }
        public IDbSet<ClientDeliveryAddress> ClientDeliveryAddress { get; set; }
        public IDbSet<Order> Order { get; set; }
        public IDbSet<OrderSub> OrderSub { get; set; }
        public IDbSet<OrderSubChild> OrderSubChild { get; set; }
        public IDbSet<OrderSubChildUnit> OrderSubChildUnit { get; set; }
        public IDbSet<OrderNotifyLog> OrderNotifyLog { get; set; }
        public IDbSet<AppSoftware> AppSoftware { get; set; }
        public IDbSet<Machine> Machine { get; set; }
        public IDbSet<MachineOperateLog> MachineOperateLog { get; set; }
        public IDbSet<MachineBindLog> MachineBindLog { get; set; }
        public IDbSet<OrderPickupLog> OrderPickupLog { get; set; }
        //public IDbSet<RptOrder> RptOrder { get; set; }
        //public IDbSet<RptOrderDetails> RptOrderDetails { get; set; }
        //public IDbSet<RptOrderDetailsChild> RptOrderDetailsChild { get; set; }
        public IDbSet<AppTraceLog> AppTraceLog { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
