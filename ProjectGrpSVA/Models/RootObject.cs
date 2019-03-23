using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProjectGrpSVA.Models
{
    public class Output1
    {
        public string rhs { get; set; }
        public string confidence { get; set; }
        public string lift { get; set; }
        public string support { get; set; }
    }

    public class Results
    {
        public List<Output1> output1 { get; set; }
    }

    public class RootObject
    {
        public Results Results { get; set; }
    }

    public class InputBasketData
    {
        public string firstInput { get; set; }
        public string secondInput { get; set; }
    }
}