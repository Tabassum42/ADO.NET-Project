using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_5
{
    public class ConnectionHP
    {
        public static string ConnectionString
        {
            get
            {
                string dbPath = Path.Combine(Path.GetFullPath(@"..\..\"), "Shop.mdf");
                return $@"Data Source=(localdb)\mssqllocaldb;AttachDbFilename={dbPath};Initial Catalog=Shop;Trusted_Connection=True";
            }
        }
    }
}
