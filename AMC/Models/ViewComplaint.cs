using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMC.Models
{
    public class ViewComplaint
    {
        public int user_id { get; set; }
        public int complaint_id { get; set; }
        public string fname { get; set; }
        public string status { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public string plan_name { get; set; }
        public string description { get; set; }
        public string sp_fname { get; set; }
        public string sp_lname { get; set; }
    }
}