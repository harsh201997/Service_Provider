using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMC.Models
{
    public class PlanEnroll
    {
        public int planenroll_id { get; set; }
        public int plan_id { get; set; }
        public int sp_id { get; set; }
        public int user_id { get; set; }
        public string plan_status { get; set; }
        public string plan_name { get; set; }
        public Nullable<System.DateTime> plan_date { get; set; }
        public Nullable<System.DateTime> planend_date { get; set; }
        public string description { get; set; }
        public string sp_fname { get; set; }
        public string sp_lname { get; set; }
        public string sp_logo { get; set; }
        public string address { get; set; }
        public Nullable<decimal> charges { get; set; }
        public string plan_duration { get; set; }
        public string plan_benifits { get; set; }
    }
}