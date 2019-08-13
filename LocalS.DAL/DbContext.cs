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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
