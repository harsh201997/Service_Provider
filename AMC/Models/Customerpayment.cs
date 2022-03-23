using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMC.Models
{
    public class Customerpayment
    {
        public int planmaintain_id { get; set; }
        public Nullable<int> plan_id { get; set; }
        public Nullable<int> planenroll_id { get; set; }
        public Nullable<int> user_id { get; set; }
        public Nullable<System.DateTime> service_date { get; set; }
        public string status { get; set; }
        public Nullable<decimal> remainpayment { get; set; }
        public Nullable<decimal> doneservice { get; set; }
        public Nullable<decimal> installment_amount { get; set; }
        public Nullable<int> sp_id { get; set; }
        public string plan_name { get; set; }
        public string sp_fname { get; set; }
        public string sp_lname { get; set; }
        public Nullable<decimal> charges { get; set; }
        public Nullable <int>no_of_services { get; set; }
        public Nullable<decimal> sp_mobile { get; set; }
    }
}