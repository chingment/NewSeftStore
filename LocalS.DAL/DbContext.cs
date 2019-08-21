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

        public IDbSet<ProductSku> ProductSku { get; set; }
        public IDbSet<ProductKind> ProductKind { get; set; }
        public IDbSet<ProductSubject> ProductSubject { get; set; }

        public IDbSet<ProductSkuKind> ProductSkuKind { get; set; }

        public IDbSet<ProductSkuSubject> ProductSkuSubject { get; set; }

        public IDbSet<Store> Store { get; set; }
        public IDbSet<StoreSellChannel> StoreSellChannel { get; set; }
        public IDbSet<StoreSellChannelStock> StoreSellChannelStock { get; set; }

        public IDbSet<ClientCart> ClientCart { get; set; }

        public IDbSet<ClientCoupon> ClientCoupon { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
