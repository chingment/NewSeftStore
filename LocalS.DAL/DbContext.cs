using LocalS.Entity;
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
        public IDbSet<MerchDevice> MerchDevice { get; set; }
        public IDbSet<MerchOrg> MerchOrg { get; set; }
        public IDbSet<PrdKind> PrdKind { get; set; }
        //public IDbSet<PrdSubject> PrdSubject { get; set; }
        public IDbSet<PrdSpu> PrdSpu { get; set; }
        public IDbSet<PrdSku> PrdSku { get; set; }
        //public IDbSet<PrdProductKind> PrdProductKind { get; set; }
        //public IDbSet<PrdProductSubject> PrdProductSubject { get; set; }
        public IDbSet<Store> Store { get; set; }
        public IDbSet<SellChannelStock> SellChannelStock { get; set; }
        public IDbSet<SellChannelStockLog> SellChannelStockLog { get; set; }
        public IDbSet<SellChannelStockDateHis> SellChannelStockDateHis { get; set; }
        public IDbSet<ClientCart> ClientCart { get; set; }
        public IDbSet<ClientCoupon> ClientCoupon { get; set; }
        public IDbSet<ClientReffSku> ClientReffSku { get; set; }
        public IDbSet<AdContent> AdContent { get; set; }
        public IDbSet<AdContentBelong> AdContentBelong { get; set; }
        public IDbSet<AdSpace> AdSpace { get; set; }
        public IDbSet<ClientDeliveryAddress> ClientDeliveryAddress { get; set; }
        public IDbSet<Order> Order { get; set; }
        public IDbSet<OrderSub> OrderSub { get; set; }
        public IDbSet<PayTrans> PayTrans { get; set; }
        public IDbSet<PayTransSub> PayTransSub { get; set; }
        public IDbSet<PayRefund> PayRefund { get; set; }
        public IDbSet<PayRefundSku> PayRefundSku { get; set; }
        
        public IDbSet<PayNotifyLog> PayNotifyLog { get; set; }
        public IDbSet<AppSoftware> AppSoftware { get; set; }
        public IDbSet<Device> Device { get; set; }
        public IDbSet<DeviceCabinet> DeviceCabinet { get; set; }
        public IDbSet<MerchOperateLog> MerchOperateLog { get; set; }
        public IDbSet<DeviceBindLog> DeviceBindLog { get; set; }
        public IDbSet<OrderPickupLog> OrderPickupLog { get; set; }
        public IDbSet<StoreKind> StoreKind { get; set; }
        public IDbSet<StoreKindSpu> StoreKindSpu { get; set; }
        //public IDbSet<RptOrder> RptOrder { get; set; }
        //public IDbSet<RptOrderDetails> RptOrderDetails { get; set; }
        //public IDbSet<RptOrderDetailsChild> RptOrderDetailsChild { get; set; }
        public IDbSet<AppTraceLog> AppTraceLog { get; set; }
        public IDbSet<Coupon> Coupon { get; set; }
        public IDbSet<CouponUseAreaObj> CouponUseAreaObj { get; set; }
        public IDbSet<CouponRevPosSt> CouponRevPosSt { get; set; }
        public IDbSet<CouponRevCouponSt> CouponRevCouponSt { get; set; }
        public IDbSet<MemberFeeSt> MemberFeeSt { get; set; }
        public IDbSet<MemberLevelSt> MemberLevelSt { get; set; }
        public IDbSet<MemberCouponSt> MemberCouponSt { get; set; }
        public IDbSet<MemberSkuSt> MemberSkuSt { get; set; }
        public IDbSet<MemberDaySt> MemberDaySt { get; set; }
        public IDbSet<SaleOutlet> SaleOutlet { get; set; }
        public IDbSet<Supplier> Supplier { get; set; }
        public IDbSet<RentOrder> RentOrder { get; set; }
        public IDbSet<RentOrderTransRecord> RentOrderTransRecord { get; set; }
        public IDbSet<StoreShop> StoreShop { get; set; }
        public IDbSet<Shop> Shop { get; set; }
        //public IDbSet<StoreSelfPickAddress> StoreSelfPickAddress { get; set; }
        public IDbSet<BI_AppTraceLog> BI_AppTraceLog { get; set; }
        public IDbSet<WxACode> WxACode { get; set; }
        public IDbSet<SvUser> SvUser { get; set; }
        public IDbSet<SvUserDevice> SvUserDevice { get; set; }
        public IDbSet<SvArticle> SvArticle { get; set; }
        public IDbSet<SvHealthDayReport> SvHealthDayReport { get; set; }
        public IDbSet<SvHealthStageReport> SvHealthStageReport { get; set; }
        public IDbSet<SvHealthDayReportLabel> SvHealthDayReportLabel { get; set; }
        public IDbSet<SvHealthDayReportAdvice> SvHealthDayReportAdvice { get; set; }
        public IDbSet<SvHealthStageReportSugSku> SvHealthStageReportSugSku { get; set; }
        public IDbSet<SvHealthStageReportTag> SvHealthStageReportTag { get; set; }
        public IDbSet<SvVisitRecord> SvVisitRecord { get; set; }
        public IDbSet<SvMerch> SvMerch { get; set; }
        public IDbSet<DeviceMqttMessage> DeviceMqttMessage { get; set; }
        public IDbSet<SvHealthTagExplain> SenvivHealthTagExplain { get; set; }
        public IDbSet<ErpReplenishPlan> ErpReplenishPlan { get; set; }
        public IDbSet<ErpReplenishPlanDevice> ErpReplenishPlanDevice { get; set; }
        public IDbSet<ErpReplenishPlanDeviceDetail> ErpReplenishPlanDeviceDetail { get; set; }
        public IDbSet<SvTask> SvTask { get; set; }
        public IDbSet<SvUserWomen> SvUserWomen { get; set; }
        public IDbSet<PushMessageLog> PushMessageLog { get; set; }
        

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
