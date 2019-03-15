using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectGrpSVA.DataAccess
{
    public class Aisle
    {
        [Key]
        public int aisle_id { get; set; }
        public string aisle { get; set; }

    }
}