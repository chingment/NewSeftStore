using System;
using System.Reflection;
using System.Text;
using System.Linq;
using LocalS.DAL;

namespace LocalS.BLL
{
    public abstract class BaseDbContext
    {
        private DbContext _CurrentDb;

        public BaseDbContext()
        {
          
        }

        protected DbContext CurrentDb
        {
            get
            {
                if (_CurrentDb == null)
                {
                    _CurrentDb = new DbContext();
                }

                return _CurrentDb;
            }
        }
    }
}
