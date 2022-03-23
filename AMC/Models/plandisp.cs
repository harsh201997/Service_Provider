using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMC.Models
{
    public class plandisp
    {

        public int planenroll_id { get; set; }
        public Nullable<int> user_id { get; set; }
        public Nullable<int> sp_id { get; set; }
        public string sp_fname { get; set; }
        public string sp_lname { get; set; }
        public string plan_status { get; set; }
        public Nullable<int> plan_id { get; set; }
        public Nullable<System.DateTime> plan_date { get; set; }
        public Nullable<System.DateTime> planend_date { get; set; }
        public string description { get; set; }
        public string plan_name { get; set; }
        public string fname { get; set; }
        public Nullable<decimal> charges { get; set; }
    }
}