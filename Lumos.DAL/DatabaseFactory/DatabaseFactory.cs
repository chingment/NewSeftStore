using System;


namespace Lumos.DbRelay
{
    public class DatabaseFactory
    {

        public static IDBOptionBySqlSentence GetIDBOptionBySql()
        {
        
            DBOptionBySqlSentenceProvider dbo = new DBOptionBySqlSentenceProvider();
            return dbo;
        }

    }
}
