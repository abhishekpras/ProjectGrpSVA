using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectGrpSVA.DataAccess
{
    public class Order_Products
    {
        public int Order_ProductsId { get; set; }
        public int order_id { get; set; }
        public int product_id { get; set; }
        public int add_to_cart_order { get; set; }
        public int reordered { get; set; }

    }
}