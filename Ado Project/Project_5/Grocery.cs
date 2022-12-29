using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_5
{
    public class Grocery
    {
        public int GroceryId { get; set; }
        public string ProductName { get; set; }
        public string Quantity { get; set; }
        public DateTime ManufactureDate { get; set; }
        public decimal Price { get; set; }
        public bool Available { get; set; }
        public string Picture { get; set; }
    }
}
