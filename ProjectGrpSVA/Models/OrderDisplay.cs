using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectGrpSVA.Models
{
    public class OrderDisplay
    {
        public int Order_Id { get; set;}
        public string Product_Name { get; set; }
        public int Add_To_Cart_Order { get; set; }
        public int Reordered { get; set; }
    }
}