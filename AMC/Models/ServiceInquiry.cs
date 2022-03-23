using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMC.Models
{
    public class ServiceInquiry
    {
        public int user_id { get; set; }
        public int sp_id { get; set; }
        public int userreq_id { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string email { get; set; }
        public string address { get; set; }
        public string description { get; set; }
        public string status { get; set; }
        public string sp_fname { get; set; }
        public string sp_lname { get; set; }
        public Nullable<decimal> sp_mobile { get; set; }
        public string sptype_name { get; set; }
        public Nullable<decimal> mobile { get; set; }
        public Nullable<decimal> token { get; set; }
        public Nullable<System.DateTime> date { get; set; }
        public Nullable<System.DateTime> processdate { get; set; }
        public Nullable<System.DateTime> enddate { get; set; }
        public Nullable<decimal> amount { get; set; }
    }
}