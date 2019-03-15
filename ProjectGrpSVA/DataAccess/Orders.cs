using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectGrpSVA.DataAccess
{
    public class Orders
    {
        [Key]
        public int order_id { get; set; }
        public int user_id { get; set; }
        public string eval_set { get; set; }
        public int order_number { get; set; }
        public int order_dow { get; set; }
        public int order_hour_of_day { get; set; }
        public int days_since_prior_order { get; set; }

    }
}