using AMC.EDM;
using AMC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AMC.Controllers
{
    public class UserController : Controller
    {
        DBServiceProviderEntities dc = new DBServiceProviderEntities();

        public ActionResult Dashboard()
        {
            int id = int.Parse(Session["user_id"].ToString());

            var Enrollplan = dc.Plan_Enroll.Where(c => c.user_id == id).Count();
            ViewBag.cnteplan = Enrollplan;

            var totcomp = dc.User_complaint.Where(c => c.user_id == id).OrderByDescending(c => c.status).Count();
            ViewBag.cntcomplaint = totcomp;

            var pendinginq = (from n in dc.Serviceprovider_regis
                              from k in dc.ServiceproviderTypes
                              from m in dc.user_request

                              where n.sp_id == m.sp_id && m.user_id == id && n.sptype_id == k.sptype_id && m.status == "Pending"
                              select new { n.sp_fname, n.sp_lname, k.sptype_name, n.sp_mobile, m.userreq_id, m.status, m.description, m.date }).OrderByDescending(c => c.status).Count();

            ViewBag.PendInq = pendinginq;


            var Inprocessinq = (from n in dc.User_regis

                                from m in dc.user_request

                                where n.user_id == m.user_id && m.sp_id == id && m.status == "In Process"
                                select new { n.fname, n.lname, n.user_id, m.userreq_id, n.mobile, n.email, n.address, m.status, m.description, m.date, m.token }).Count();
            ViewBag.inprocessinq = Inprocessinq;
            return View();

        }
        public ActionResult Index()
        {
            return View();

        }
        public ActionResult ViewPlan()
        {
            var p = dc.Tbl_plan.ToList();
            ViewData["temp"] = p;
            return View();
        }
        public ActionResult PlanDetail(int id)
        {
            var p = dc.Tbl_plan.Find(id);
            ViewData["temp"] = p;
            return View();

        }
        [HttpPost]
        public ActionResult PlanDetail(FormCollection fc)
        {
            int user_id = int.Parse(Session["user_id"].ToString());
            int planid = int.Parse(fc["pid"].ToString());
            int sp_id = int.Parse(fc["sp_id"].ToString());
            string plan_description = fc["pdesc"];
            string stardate = DateTime.Now.ToShortDateString();
            string enddate = "";
            string duration = fc["duration"];

            if (duration == "1 year")
            {
                enddate = DateTime.Now.AddDays(365).ToShortDateString();
            }
            else if (duration == "6 months")
            {
                enddate = DateTime.Now.AddDays(185).ToShortDateString();
            }
            else if (duration == "4 months")
            {
                enddate = DateTime.Now.AddDays(124).ToShortDateString();
            }

            var p = (from n in dc.Plan_Enroll where n.plan_id == planid && n.user_id == user_id select n).ToList().Count();
            if (p == 1)
            {
                ViewBag.msg = "!...you Already Enrolled this Plan";
                return RedirectToAction("ViewPlan");

            }
            else
            {
                Plan_Enroll obj = new Plan_Enroll();
                obj.plan_id = planid;
                obj.plan_status = "Running";
                obj.sp_id = sp_id;
                obj.user_id = user_id;
                obj.plan_date = DateTime.Parse(stardate);
                obj.planend_date = DateTime.Parse(enddate);

                obj.description = plan_description;
                dc.Plan_Enroll.Add(obj);
                dc.SaveChanges();
                ViewBag.msg = "Plan Successfully Enrolled..";

                return RedirectToAction("ViewPlan");
            }


        }
        public ActionResult AddReview(int id)
        {
            ViewBag.spid = id;

            return View();
        }
        [HttpPost]
        public ActionResult AddReview(FormCollection fc)
        {

            int spid = int.Parse(fc["spid"].ToString());

            var reqid = dc.user_request.Where(c => c.sp_id == spid).FirstOrDefault().userreq_id;
            string desc = fc["desc"];
            User_review obj = new User_review();
            obj.review_desc = desc;
            obj.sp_id = spid;
            obj.userreq_id = reqid;
            dc.User_review.Add(obj);
            dc.SaveChanges();
            return RedirectToAction("ViewCompletedInquiry");

        }

        public ActionResult Complaint()
        {
            int user_id = int.Parse(Session["user_id"].ToString());
            var s = (from n in dc.Plan_Enroll

                     from m in dc.Tbl_plan
                     where n.plan_id == m.plan_id && n.user_id == user_id
                     select new { n.plan_id, m.plan_name }).ToList();

            List<UComplaint> list = new List<UComplaint>();

            foreach (var item in s)
            {
                UComplaint obj = new UComplaint();
                obj.plan_id = int.Parse(item.plan_id.ToString());
                obj.plan_name = item.plan_name;
                list.Add(obj);
            }

            ViewData["temp"] = list;
            return View();
        }
        [HttpPost]
        public ActionResult Complaint(FormCollection fc)
        {
            int user_id = int.Parse(Session["user_id"].ToString());


            var s = (from n in dc.Plan_Enroll

                     from m in dc.Tbl_plan
                     where n.plan_id == m.plan_id && n.user_id == user_id
                     select new { n.plan_id, m.plan_name }).ToList();

            List<UComplaint> list = new List<UComplaint>();

            foreach (var item in s)
            {
                UComplaint obj = new UComplaint();
                obj.plan_id = int.Parse(item.plan_id.ToString());
                obj.plan_name = item.plan_name;

                list.Add(obj);
            }


            ViewData["temp"] = list;

            string desc = fc["desc"];
            string plan_id = fc["plan"];
            User_complaint obj2 = new User_complaint();
            obj2.user_id = user_id;
            obj2.plan_id = int.Parse(plan_id.ToString());

            obj2.date = DateTime.Parse(DateTime.Now.ToShortDateString());
            obj2.status = "pending";
            obj2.description = desc;
            dc.User_complaint.Add(obj2);
            dc.SaveChanges();
            ViewBag.msg = "Complaint Registered Successfully";

            return View();


        }
        public ActionResult ViewPlanComplaint()
        {
            int user_id = int.Parse(Session["user_id"].ToString());

            var s = (from n in dc.User_complaint
                     from l in dc.User_regis
                     from m in dc.Tbl_plan
                     from k in dc.Serviceprovider_regis
                     where n.user_id == l.user_id && n.plan_id == m.plan_id && m.sp_id == k.sp_id && n.user_id == user_id
                     select new { n.date, n.description, n.status, n.user_id, n.complaint_id, l.fname, m.plan_name, k.sp_fname, k.sp_lname }).OrderByDescending(c => c.status).ToList();

            List<ViewComplaint> list = new List<ViewComplaint>();

            foreach (var item in s)
            {
                ViewComplaint obj = new ViewComplaint();
                obj.fname = item.fname;
                obj.status = item.status;
                obj.plan_name = item.plan_name;
                obj.description = item.description;
                obj.date = item.date;
                obj.user_id = int.Parse(item.user_id.ToString());
                obj.complaint_id = item.complaint_id;
                obj.sp_fname = item.sp_fname;
                obj.sp_lname = item.sp_lname;
                list.Add(obj);
            }


            ViewData["temp"] = list;
            return View();

        }

        public ActionResult ViewPlanEnrolled()
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
        public ActionResult PlanEnrolledDetail(int id)
        {
            int user_id = int.Parse(Session["user_id"].ToString());
            var s = (from n in dc.Plan_Enroll
                     from l in dc.Serviceprovider_regis
                     from m in dc.Tbl_plan

                     where n.sp_id == l.sp_id && n.plan_id == m.plan_id && n.user_id == user_id && n.planenroll_id == id
                     select new { n.planenroll_id, m.charges, l.sp_id, l.sp_lname, m.plan_benifits, n.description, l.address, m.plan_duration, n.user_id, l.sp_fname, }).ToList();

            List<PlanEnroll> list = new List<PlanEnroll>();
            foreach (var item in s)
            {
                PlanEnroll obj = new PlanEnroll();


                obj.sp_id = int.Parse(item.sp_id.ToString());

                obj.user_id = int.Parse(item.user_id.ToString());

                obj.sp_fname = item.sp_fname;
                obj.sp_lname = item.sp_lname;
                obj.plan_benifits = item.plan_benifits;
                obj.description = item.description;
                obj.planenroll_id = item.planenroll_id;
                obj.charges = decimal.Parse(item.charges.ToString());
                obj.address = item.address;
                obj.plan_duration = item.plan_duration;
                list.Add(obj);
            }

            ViewData["temp"] = list;
            return View();


        }
        public JsonResult getdata(int sid)
        {
            var p = (from n in dc.Serviceprovider_regis
                     where n.sptype_id == sid
                     select new { n.sp_id, n.sp_fname }).ToList();

            return Json(p, JsonRequestBehavior.AllowGet);

        }

        public JsonResult gdata(int spid)
        {
            var p = (from n in dc.Charge_rate
                     where n.sp_id == spid
                     select new { n.amount }).ToList();
            return Json(p, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ServiceInquiry()
        {
            var p = dc.ServiceproviderTypes.ToList();
            ViewData["temp"] = p;
            return View();

        }
        [HttpPost]
        public ActionResult ServiceInquiry(FormCollection fc)
        {
            var p = dc.ServiceproviderTypes.ToList();
            ViewData["temp"] = p;

            int sptype = int.Parse(fc["sptype"].ToString());
            string description = fc["desc"].ToString();
            string servicedetail = fc["sdetail"].ToString();
            string charge = fc["charge"];
            int spid = int.Parse(fc["sprovider"].ToString());
            int userid = int.Parse(Session["user_id"].ToString());

            user_request obj = new user_request();
            obj.sptype_id = sptype;
            obj.user_id = userid;
            obj.date = DateTime.Now;
            obj.description = description;
            obj.req_data = servicedetail;
            obj.sp_id = spid;
            obj.status = "Pending";
            dc.user_request.Add(obj);
            dc.SaveChanges();
            ViewBag.msg = "Inquiry successfully Recorded... ";
            return View();
        }

        public ActionResult ViewPendingInquiry()
        {
            int userid = int.Parse(Session["user_id"].ToString());
            var s = (from n in dc.Serviceprovider_regis
                     from k in dc.ServiceproviderTypes
                     from m in dc.user_request

                     where n.sp_id == m.sp_id && m.user_id == userid && n.sptype_id == k.sptype_id && m.status == "Pending"
                     select new { n.sp_fname, n.sp_lname, k.sptype_name, n.sp_mobile, m.userreq_id, m.status, m.description, m.date }).OrderByDescending(c => c.status).ToList();

            List<ServiceInquiry> list = new List<ServiceInquiry>();

            foreach (var item in s)
            {
                ServiceInquiry obj = new ServiceInquiry();
                obj.sp_fname = item.sp_fname;
                obj.sp_lname = item.sp_lname;
                obj.sp_mobile = item.sp_mobile;
                obj.sptype_name = item.sptype_name;
                obj.status = item.status;

                obj.date = item.date;
                obj.description = item.description;


                obj.userreq_id = item.userreq_id;

                list.Add(obj);
            }


            ViewData["temp"] = list;
            return View();
        }

        public ActionResult Inprocessinquiry()
        {
            int userid = int.Parse(Session["user_id"].ToString());
            var s = (from n in dc.Serviceprovider_regis
                     from k in dc.ServiceproviderTypes
                     from m in dc.user_request

                     where n.sp_id == m.sp_id && m.user_id == userid && n.sptype_id == k.sptype_id && m.status == "In Process"
                     select new { n.sp_fname, n.sp_lname, k.sptype_name, n.sp_mobile, m.processdate, m.userreq_id, m.status, m.description, m.date }).OrderByDescending(c => c.status).ToList();

            List<ServiceInquiry> list = new List<ServiceInquiry>();

            foreach (var item in s)
            {
                ServiceInquiry obj = new ServiceInquiry();
                obj.sp_fname = item.sp_fname;
                obj.sp_lname = item.sp_lname;
                obj.sp_mobile = item.sp_mobile;
                obj.sptype_name = item.sptype_name;
                obj.status = item.status;
                obj.processdate = item.processdate;
                obj.date = item.date;
                obj.description = item.description;


                obj.userreq_id = item.userreq_id;

                list.Add(obj);
            }


            ViewData["temp"] = list;
            return View();
        }
        public ActionResult ViewCompletedInquiry()
        {
            int userid = int.Parse(Session["user_id"].ToString());
            var s = (from n in dc.Serviceprovider_regis
                     from k in dc.ServiceproviderTypes
                     from m in dc.user_request

                     where n.sp_id == m.sp_id && m.user_id == userid && n.sptype_id == k.sptype_id && m.status == "Done"
                     select new { n.sp_fname, n.sp_lname, n.sp_id, k.sptype_name, n.sp_mobile, m.processdate, m.enddate, m.userreq_id, m.status, m.description, m.date }).OrderByDescending(c => c.status).ToList();

            List<ServiceInquiry> list = new List<ServiceInquiry>();

            foreach (var item in s)
            {
                ServiceInquiry obj = new ServiceInquiry();
                obj.sp_fname = item.sp_fname;
                obj.sp_lname = item.sp_lname;
                obj.sp_mobile = item.sp_mobile;
                obj.sptype_name = item.sptype_name;
                obj.status = item.status;
                obj.sp_id = item.sp_id;
                obj.date = item.date;
                obj.processdate = item.processdate;
                obj.enddate = item.enddate;
                obj.description = item.description;


                obj.userreq_id = item.userreq_id;

                list.Add(obj);
            }


            ViewData["temp"] = list;
            return View();
        }

        public ActionResult ViewPlanPayment()
        {


            int user_id = int.Parse(Session["user_id"].ToString());
            //

            var dta = (from n in dc.Plan_Enroll

                       from m in dc.Tbl_plan
                       where n.plan_id == m.plan_id && n.user_id == user_id
                       select new { n.plan_id, m.plan_name }).ToList();

            List<UComplaint> li = new List<UComplaint>();

            foreach (var item in dta)
            {
                UComplaint obj1 = new UComplaint();
                obj1.plan_id = int.Parse(item.plan_id.ToString());
                obj1.plan_name = item.plan_name;

                li.Add(obj1);
            }


            ViewData["temp2"] = li;

            return View();
        }


        public JsonResult getdata3(int cid)
        {
            int user_id = int.Parse(Session["user_id"].ToString());
            var s = (from n in dc.Plan_maintanance
                     from l in dc.Serviceprovider_regis
                     from k in dc.Plan_Enroll
                     from m in dc.Tbl_plan
                     where n.planenroll_id == k.planenroll_id && k.sp_id == l.sp_id && n.plan_id == m.plan_id && n.user_id == user_id && n.plan_id == cid
                     select new { l.sp_fname, l.sp_lname, n.service_date, m.no_of_services, n.installment_amount, m.charges, l.sp_mobile, n.user_id, m.sp_id, m.plan_name, m.plan_id }).ToList();



            var cnt = (from n in dc.Plan_maintanance
                       from l in dc.Serviceprovider_regis
                       from k in dc.Plan_Enroll
                       from m in dc.Tbl_plan
                       where n.planenroll_id == k.planenroll_id && k.sp_id == l.sp_id && n.plan_id == m.plan_id && n.user_id == user_id && n.plan_id == cid
                       select new { l.sp_fname, l.sp_lname, n.service_date, m.no_of_services, n.installment_amount, m.charges, l.sp_mobile, n.user_id, m.sp_id, m.plan_name, m.plan_id }).ToList().Count();
            if (cnt == 0)
            {
                cnt = 1;

            }
            List<Customerpayment> list = new List<Customerpayment>();

            decimal total = 0;
            decimal remain = 0;
            decimal sumass = 0;
            foreach (var item in s)
            {
                Customerpayment obj = new Customerpayment();
                obj.sp_fname = item.sp_fname;
                obj.sp_lname = item.sp_lname;
                obj.plan_name = item.plan_name;
                obj.no_of_services = item.no_of_services;
                obj.charges = item.charges;
                obj.installment_amount = item.installment_amount;

                sumass = decimal.Parse((sumass + item.installment_amount).ToString());

                obj.remainpayment = decimal.Parse(item.charges.ToString()) - sumass;

                obj.sp_mobile = item.sp_mobile;
                obj.service_date = item.service_date;

                total = decimal.Parse((total + item.installment_amount).ToString());
                remain = decimal.Parse((item.charges - total).ToString());

                list.Add(obj);
            }


            ViewBag.total = total;
            ViewBag.rem = remain;

            return Json(list, JsonRequestBehavior.AllowGet);

        }
    }
}
