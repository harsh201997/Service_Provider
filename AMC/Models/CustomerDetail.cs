using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMC.Models
{
    public class CustomerDetail
    {
        public string fname { get; set; }
        public string lname { get; set; }
        public string email { get; set; }
        public Nullable<decimal> mobile { get; set; }
        public string address { get; set; }
        public string user_img { get; set; }
    }
}