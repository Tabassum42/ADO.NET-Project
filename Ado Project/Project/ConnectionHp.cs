using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    public static class ConnectionHp
    {
        public static string ConnectionString
        {
            get
            {
                string dbPath = Path.Combine(Path.GetFullPath(@"..\..\"), "StoreDB.mdf");
                return $@"Data Source=(localdb)\mssqllocaldb;AttachDbFilename={dbPath};Initial Catalog=BooksDb;Trusted_Connection=True";
            }
        }
    }
}
