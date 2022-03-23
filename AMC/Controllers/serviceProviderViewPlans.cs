using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMC.EDM;
using AMC.Models;

namespace TemplatePattern
{
    public class serviceProviderViewPlans : viewPlanTemplate
    {

        protected override void viewAllPlans()
        {
            DBServiceProviderEntities dc = new DBServiceProviderEntities();
            var p = dc.Tbl_plan.ToList();
            ViewData["temp"] = p;

        }

        protected override void viewMyPlans()
        {
            int spid = int.Parse(Session["sp_id"].ToString());
            var p = (from n in dc.Tbl_plan where n.sp_id == spid select n).ToList();
            // var p = dc.Tbl_plan.ToList();
            ViewData["temp"] = p;
            return View();
        }
    }
}
