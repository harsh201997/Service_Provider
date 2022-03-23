using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AMC.EDM;
using AMC.Models;

namespace TemplatePattern
{
    public class customerViewPlans : viewPlanTemplate
    {
        DBServiceProviderEntities dc = new DBServiceProviderEntities();

        public ActionResult viewAllPlans()
        {
            var p = dc.Tbl_plan.ToList();
            ViewData["temp"] = p;
            return View();
        }

        public ActionResult viewMyPlans()
        {
            int user_id = int.Parse(Session["user_id"].ToString());
            var s = (from n in dc.Plan_Enroll
                     from l in dc.Serviceprovider_regis
                     from m in dc.Tbl_plan

                     where n.sp_id == l.sp_id && n.plan_id == m.plan_id && n.user_id == user_id
                     select new { n.plan_date, n.planend_date, n.planenroll_id, n.plan_status, l.sp_id, l.sp_lname, m.plan_benifits, n.description, n.user_id, l.sp_fname, l.sp_mobile, l.sp_logo, m.plan_name }).ToList();
           
            List<PlanEnroll> list = new List<PlanEnroll>();
            foreach (var item in s)
            {
                PlanEnroll obj = new PlanEnroll();
                obj.plan_date = item.plan_date;
                obj.plan_status = item.plan_status;
                obj.sp_id = int.Parse(item.sp_id.ToString());
                obj.planend_date = item.planend_date;
                obj.user_id = int.Parse(item.user_id.ToString());
                obj.plan_name = item.plan_name;
                obj.sp_logo = item.sp_logo;
                obj.sp_fname = item.sp_fname;
                obj.sp_lname = item.sp_lname;
                obj.plan_name = item.plan_name;
                obj.plan_benifits = item.plan_benifits;
                obj.description = item.description;
                obj.planenroll_id = item.planenroll_id;
                list.Add(obj);
            }

            ViewData["temp"] = list;
            return View();
        }
    }
}
