using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProjectGrpSVA.DataAccess
{
    public class Departments
    {
        [Key]
        public int department_id { get; set; }
        public string department { get; set; }
    }
}