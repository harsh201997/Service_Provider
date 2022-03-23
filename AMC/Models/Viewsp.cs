using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AMC.Models
{
    public class Viewsp
    {
        public int sp_id { get; set; }
        public string sp_fname { get; set; }
        public string sp_lname { get; set; }
        public string sp_logo { get; set; }
        public string sp_gender { get; set; }
        public string sp_email { get; set; }
        public Nullable<decimal> sp_mobile { get; set; }
        public Nullable<int> sptype_id { get; set; }
        public Nullable<System.DateTime> spregis_date { get; set; }
        public string sp_status { get; set; }
        public string address { get; set; }
        public string sptype_name { get; set; }
    }
}