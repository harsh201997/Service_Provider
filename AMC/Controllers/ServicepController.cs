using AMC.EDM;
using AMC.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
namespace AMC.Controllers
{
    public class ServicepController : Controller
    {

        DBServiceProviderEntities dc = new DBServiceProviderEntities();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            int id = int.Parse(Session["sp_id"].ToString());

            var sttotal = dc.Tbl_plan.Where(c => c.sp_id == id).Count();
            ViewBag.cnt = sttotal;

            var s = (from n in dc.Plan_Enroll
                     from l in dc.User_regis
                     from m in dc.Tbl_plan
                     where n.user_id == l.user_id && n.plan_id == m.plan_id && n.sp_id == id
                     select new { n.plan_date, n.plan_status, n.planenroll_id, n.sp_id, n.planend_date, n.user_id, l.fname, m.plan_name }).ToList();
            var sp = s.GroupBy(test => test.user_id).Select(grp => grp.First()).ToList().Count();
            ViewBag.cntuser = sp;

            var totcomplaint = (from n in dc.User_complaint
                                from l in dc.User_regis
                                from m in dc.Tbl_plan

                                where n.user_id == l.user_id && n.plan_id == m.plan_id && m.sp_id == id && n.status == "Pending"
                                select new { n.date, n.description, n.status, n.user_id, n.complaint_id, l.fname, m.plan_name }).Count();
            ViewBag.totcomp = totcomplaint;

            var todaycomp = (from n in dc.User_complaint
                             from l in dc.User_regis
                             from m in dc.Tbl_plan

                             where n.user_id == l.user_id && n.plan_id == m.plan_id && m.sp_id == id && n.status == "Pending"
                             && (n.date.Value.Year == DateTime.Now.Year && n.date.Value.Month == DateTime.Now.Month && n.date.Value.Day == DateTime.Now.Day)
                             select new { n.date, n.description, n.status, n.user_id, n.complaint_id, l.fname, m.plan_name }).Count();
            ViewBag.tottodaycomp = todaycomp;


            var pendinginquiry = (from n in dc.User_regis

                                  from m in dc.user_request

                                  where n.user_id == m.user_id && m.sp_id == id && m.status == "Pending"
                                  select new { n.fname, n.lname, n.user_id, m.userreq_id, n.mobile, n.email, n.address, m.status, m.description, m.date, m.token }).Count();

            ViewBag.PendInq = pendinginquiry;


            var Todayinquiry = (from n in dc.User_regis

                                from m in dc.user_request

                                where n.user_id == m.user_id && m.sp_id == id && m.status == "Pending" && (m.date.Value.Year == DateTime.Now.Year && m.date.Value.Month == DateTime.Now.Month && m.date.Value.Day == DateTime.Now.Day)
                                select new { n.fname, n.lname, n.user_id, m.userreq_id, n.mobile, n.email, n.address, m.status, m.description, m.date, m.token }).Count();
            ViewBag.TodInq = Todayinquiry;

            var Totalinquiry = (from n in dc.User_regis

                                from m in dc.user_request

                                where n.user_id == m.user_id && m.sp_id == id
                                select new { n.fname, n.lname, n.user_id, m.userreq_id, n.mobile, n.email, n.address, m.status, m.description, m.date, m.token }).Count();
            ViewBag.TotInq = Totalinquiry;


            return View();
        }
        public ActionResult AddPlan()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddPlan(FormCollection fc)
        {
            string name = fc["pname"].ToString().ToLower();
            string description = fc["pdesc"].ToString();
            string benifit = fc["benifits"].ToString();
            decimal charge = decimal.Parse(fc["charge"].ToString());
            string duration = fc["duration"].ToString();
            string spid = Session["sp_id"].ToString();
            int dur = 0;
            if (duration == "1 year")
            {
                dur = 12;
            }
            else if (duration == "6 months")
            {
                dur = 6;
            }
            else
            {
                dur = 4;
            }

            var p = (from n in dc.Tbl_plan where n.plan_duration == duration && n.plan_name == name select n).ToList().Count();

            if (p == 1)
            {
                ViewBag.msg = "Plan Already Exist";
                return View();

            }
            else
            {
                Tbl_plan obj = new Tbl_plan();
                obj.plan_name = name;
                obj.plan_description = description;
                obj.plan_benifits = benifit;
                obj.charges = decimal.Parse(charge.ToString());
                obj.plan_duration = duration;
                obj.plan_status = "Open";
                obj.no_of_services = dur / 2;
                obj.installment = charge / (dur / 2);
                obj.sp_id = int.Parse(spid.ToString());
                dc.Tbl_plan.Add(obj);
                dc.SaveChanges();
                ViewBag.msg = "Plan Successfully Added";
               
              // Configure Observer pattern
                PlanSubject s = new PlanSubject();

                // Get the list of users and subscribe
                foreach (User_regis user in dc.User_regis.ToList())
                {
                    s.AddObserver(new NotifyObserver(s, user));
                }

                // Change subject and notify observers
                s.SubjectState = "NEW PLAN ADDED";
                s.Notify();

                // Unsubscribe the users
                foreach (User_regis user in dc.User_regis.ToList())
                {
                    s.RemoveObserver(new NotifyObserver(s, user));
                }


                return View();
            }
        }

        public ActionResult ViewPlan()
        {
            int spid = int.Parse(Session["sp_id"].ToString());
            var p = (from n in dc.Tbl_plan where n.sp_id == spid select n).ToList();
            ViewData["temp"] = p;
            return View();
        }
        public ActionResult DeletePlan(int id)
        {
            var p = (from n in dc.Tbl_plan where n.plan_id == id select n).FirstOrDefault();
            dc.Tbl_plan.Remove(p);
            dc.SaveChanges();
            return RedirectToAction("ViewPlan");
        }
        public ActionResult UpdatePlan(int id)
        {
            var p = (from n in dc.Tbl_plan where n.plan_id == id select n).FirstOrDefault();
            ViewData["temp"] = p;
            return View();
        }
        [HttpPost]
        public ActionResult UpdatePlan(FormCollection fc)
        {
            string name = fc["pname"].ToString();
            string description = fc["pdesc"].ToString();
            string benifit = fc["benifits"].ToString();
            string charge = fc["charge"];
            string duration = fc["duration"].ToString();
            string spid = Session["sp_id"].ToString();
            int plan_id = int.Parse(fc["plan_id"].ToString());
            var p = (from n in dc.Tbl_plan where n.plan_id == plan_id select n).FirstOrDefault();
            p.plan_name = name;
            p.plan_description = description;
            p.plan_benifits = benifit;
            p.charges = decimal.Parse(charge.ToString());
            p.plan_duration = duration;

            dc.SaveChanges();
            ViewBag.msg = "Record Updated";
            return RedirectToAction("ViewPlan");
        }
        public ActionResult ClosedPlan(int id)
        {
            var p = dc.Tbl_plan.Find(id);
            p.plan_status = "Closed";
            dc.SaveChanges();
            return RedirectToAction("ViewPlan");
        }

        public void set()
        {

            int spid = int.Parse(Session["sp_id"].ToString());
            var p = (from n in dc.Tbl_plan where n.sp_id == spid select n).ToList();
            ViewData["temp"] = p;

            var s = (from n in dc.Plan_Enroll
                     from l in dc.User_regis
                     from m in dc.Tbl_plan
                     where n.user_id == l.user_id && n.plan_id == m.plan_id && n.sp_id == spid
                     select new { l.user_id, m.plan_name }).ToList();

            List<PlanEnroll> li = new List<PlanEnroll>();

            foreach (var item in s)
            {
                PlanEnroll obj = new PlanEnroll();
                obj.user_id = item.user_id;
                li.Add(obj);
            }


            ViewData["uid"] = li;
        }
        public ActionResult PlanMaintainance()
        {
            int spid = int.Parse(Session["sp_id"].ToString());
            var p = (from n in dc.Tbl_plan where n.sp_id == spid select n).ToList();
            ViewData["temp"] = p;

            var s = (from n in dc.Plan_Enroll
                     from l in dc.User_regis
                     from m in dc.Tbl_plan
                     where n.user_id == l.user_id && n.plan_id == m.plan_id && n.sp_id == spid
                     select new { l.user_id, m.plan_name }).ToList();

            List<PlanEnroll> li = new List<PlanEnroll>();

            foreach (var item in s)
            {
                PlanEnroll obj = new PlanEnroll();
                obj.user_id = item.user_id;
                li.Add(obj);
            }


            ViewData["uid"] = li;
            return View();


        }
        [HttpPost]
        public ActionResult PlanMaintainance(FormCollection fc)
        {

            int spid = int.Parse(Session["sp_id"].ToString());
            var p2 = (from n in dc.Tbl_plan where n.sp_id == spid select n).ToList();
            ViewData["temp"] = p2;

            var s = (from n in dc.Plan_Enroll
                     from l in dc.User_regis
                     from m in dc.Tbl_plan
                     where n.user_id == l.user_id && n.plan_id == m.plan_id && n.sp_id == spid
                     select new { l.user_id, m.plan_name }).ToList();
            ViewData["uid"] = s;

            int uid = s[0].user_id;
            int planid = int.Parse(fc["pname"].ToString());
            int enrollid = int.Parse(fc["cname"].ToString());
            string dte = fc["date"].ToString();
            string desc = fc["pdesc"].ToString();
            string charge = fc["chrge"];
            decimal installm = decimal.Parse(fc["install"].ToString());
            var k = (from n in dc.Plan_maintanance

                     where n.user_id == uid && n.plan_id == planid && n.planenroll_id == enrollid
                     select new { }).Count();

            var ss = dc.Tbl_plan.Find(planid).no_of_services;


            if (k < ss)
            {
                Plan_maintanance obj = new Plan_maintanance();
                var p = (from n in dc.Plan_Enroll where n.planenroll_id == enrollid select n);
                var user_id = p.FirstOrDefault().user_id;
                obj.plan_id = int.Parse(planid.ToString());
                obj.planenroll_id = int.Parse(enrollid.ToString());
                obj.description = desc;
                obj.user_id = user_id;
                obj.status = "Payment";
                obj.installment_amount = installm;
                obj.service_date = DateTime.Parse(dte.ToString());
                obj.extra_charge = decimal.Parse(charge.ToString());
                dc.Plan_maintanance.Add(obj);
                dc.SaveChanges();
                ViewBag.msg = "Successfully Recorded..";
                // return RedirectToAction("PlanMaintainance");

                set();
                return View();

            }
            else
            {
                set();
                ViewBag.msg = "Plan Amount already Submited";
                return View();
            }
        }

        public JsonResult getdata(int cid)
        {
            var p = (from n in dc.Plan_Enroll
                     from l in dc.User_regis
                     from s in dc.Tbl_plan
                     where n.user_id == l.user_id && n.plan_id == cid && n.plan_id == s.plan_id
                     select new { n.planenroll_id, l.fname, s.installment, n.user_id }).ToList();
            ViewBag.amt = dc.Tbl_plan.Find(cid).installment;

            List<Spmaintain> li = new List<Spmaintain>();

            foreach (var item in p)
            {
                Spmaintain obj = new Spmaintain();

                obj.fname = item.fname;
                obj.planenroll_id = item.planenroll_id;
                obj.installment = decimal.Parse(item.installment.ToString());

                li.Add(obj);
            }
            return Json(li, JsonRequestBehavior.AllowGet);

        }

        public JsonResult getdata1(int cid)
        {

            var pp1 = dc.Plan_Enroll.Where(c => c.planenroll_id == cid).ToList();

            return Json(pp1, JsonRequestBehavior.AllowGet);

        }
        public ActionResult ViewPlanEnroll()
        {
            int spid = int.Parse(Session["sp_id"].ToString());

            var s = (from n in dc.Plan_Enroll
                     from l in dc.User_regis
                     from m in dc.Tbl_plan
                     where n.user_id == l.user_id && n.plan_id == m.plan_id && n.sp_id == spid
                     select new { n.plan_date, n.plan_status, n.planenroll_id, n.sp_id, n.planend_date, n.user_id, l.fname, m.plan_name }).ToList();

            List<plandisp> list = new List<plandisp>();

            foreach (var item in s)
            {
                plandisp obj = new plandisp();
                obj.plan_date = item.plan_date;
                obj.plan_status = item.plan_status;
                obj.planenroll_id = item.planenroll_id;
                obj.sp_id = item.sp_id;
                obj.planend_date = item.planend_date;
                obj.user_id = item.user_id;
                obj.plan_name = item.plan_name;
                obj.fname = item.fname;
                list.Add(obj);
            }


            ViewData["temp"] = list;
            return View();
        }
        public ActionResult ExpiredPlanEnroll(int id)
        {
            var p = dc.Plan_Enroll.Find(id);
            p.plan_status = "Expired";
            dc.SaveChanges();
            return RedirectToAction("ViewPlanEnroll");
        }

        public ActionResult ViewCustomerDetail(int id)
        {
            int spid = int.Parse(Session["sp_id"].ToString());

            var s = (from n in dc.Plan_Enroll
                     from l in dc.User_regis

                     where n.user_id == l.user_id && n.sp_id == spid && l.user_id == id
                     select new { l.lname, l.fname, l.email, l.address, l.mobile, l.user_img }).ToList();

            List<CustomerDetail> list = new List<CustomerDetail>();

            foreach (var item in s)
            {
                CustomerDetail obj = new CustomerDetail();
                obj.lname = item.lname;
                obj.fname = item.fname;
                obj.email = item.email;
                obj.address = item.address;
                obj.mobile = item.mobile;
                obj.user_img = item.user_img;

                list.Add(obj);

            }

            ViewData["temp"] = list;
            return View();

        }
        public ActionResult ViewPlanComplaint()
        {
            int spid = int.Parse(Session["sp_id"].ToString());
            var s = (from n in dc.User_complaint
                     from l in dc.User_regis
                     from m in dc.Tbl_plan

                     where n.user_id == l.user_id && n.plan_id == m.plan_id && m.sp_id == spid
                     select new { n.date, n.description, n.status, n.user_id, n.complaint_id, l.fname, m.plan_name }).OrderByDescending(c => c.status).ToList();

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
                list.Add(obj);
            }


            ViewData["temp"] = list;
            return View();
        }

        public ActionResult ViewPendingComplaint()
        {
            int spid = int.Parse(Session["sp_id"].ToString());
            var s = (from n in dc.User_complaint
                     from l in dc.User_regis
                     from m in dc.Tbl_plan

                     where n.user_id == l.user_id && n.plan_id == m.plan_id && m.sp_id == spid && n.status == "Pending"
                     select new { n.date, n.description, n.status, n.user_id, n.complaint_id, l.fname, m.plan_name }).ToList();

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
                list.Add(obj);
            }


            ViewData["temp"] = list;
            return View();
        }
        public ActionResult TodayComplain()
        {
            int spid = int.Parse(Session["sp_id"].ToString());
            var todaycomp = (from n in dc.User_complaint
                             from l in dc.User_regis
                             from m in dc.Tbl_plan

                             where n.user_id == l.user_id && n.plan_id == m.plan_id && m.sp_id == spid && n.status == "Pending" && (n.date.Value.Year == DateTime.Now.Year && n.date.Value.Month == DateTime.Now.Month && n.date.Value.Day == DateTime.Now.Day)
                             select new { n.date, n.description, n.status, n.user_id, n.complaint_id, l.fname, m.plan_name }).ToList();
            List<ViewComplaint> list = new List<ViewComplaint>();

            foreach (var item in todaycomp)
            {
                ViewComplaint obj = new ViewComplaint();
                obj.fname = item.fname;
                obj.status = item.status;
                obj.plan_name = item.plan_name;
                obj.description = item.description;
                obj.date = item.date;
                obj.user_id = int.Parse(item.user_id.ToString());
                obj.complaint_id = item.complaint_id;
                list.Add(obj);
            }


            ViewData["temp"] = list;
            return View();
        }
        public ActionResult UserDetail(int id)
        {
            var p = dc.User_regis.Find(id);
            ViewData["Temp"] = p;
            return View();
        }
        public ActionResult DoneComplaint(int id)
        {
            var p = dc.User_complaint.Find(id);
            p.status = "Done";
            dc.SaveChanges();
            return RedirectToAction("ViewPlanComplaint");
        }

        public ActionResult AddCharge()
        {
            int spid = int.Parse(Session["sp_id"].ToString());
            var s = (from n in dc.Serviceprovider_regis
                     from l in dc.ServiceproviderTypes

                     where n.sp_id == spid && l.sptype_id == n.sptype_id
                     select new { n.sptype_id, l.sptype_name }).ToList();
            List<Viewsp> list = new List<Viewsp>();

            foreach (var item in s)
            {
                Viewsp obj = new Viewsp();
                obj.sptype_id = item.sptype_id;
                obj.sptype_name = item.sptype_name;


                list.Add(obj);
            }


            ViewData["temp"] = list;
            return View();

        }
        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }
        [HttpPost]
        public ActionResult AddCharge(FormCollection fc)
        {
            int spid = int.Parse(Session["sp_id"].ToString());
            var s = (from n in dc.Serviceprovider_regis
                     from l in dc.ServiceproviderTypes

                     where n.sp_id == spid && l.sptype_id == n.sptype_id
                     select new { n.sptype_id, l.sptype_name }).ToList();
            List<Viewsp> list = new List<Viewsp>();

            foreach (var item in s)
            {
                Viewsp obj1 = new Viewsp();
                obj1.sptype_id = item.sptype_id;
                obj1.sptype_name = item.sptype_name;


                list.Add(obj1);
            }


            ViewData["temp"] = list;

            string service = fc["sptype"].ToString();
            string amount = fc["amount"].ToString();
            string description = fc["desc"];

            try
            {

                var p = (from n in dc.Charge_rate where n.sp_id == spid select n).ToList().Count();
                if (p == 1)
                {
                    ViewBag.msg = "Amount is Already Set..";
                    return View();

                }
                else
                {
                    Charge_rate obj = new Charge_rate();
                    obj.amount = decimal.Parse(amount);
                    obj.sptype_id = int.Parse(service.ToString());
                    obj.description = description;
                    obj.sp_id = spid;
                    dc.Charge_rate.Add(obj);
                    dc.SaveChanges();
                    ViewBag.msg = "Successfully Inserted..";

                }

            }
            catch (Exception ex)
            {

                Charge_rate obj = new Charge_rate();
                obj.amount = decimal.Parse(amount);
                obj.sptype_id = int.Parse(service.ToString());
                obj.description = description;
                obj.sp_id = spid;
                dc.Charge_rate.Add(obj);
                dc.SaveChanges();
                ViewBag.msg = "Successfully Inserted..";

            }

            return View();
        }

        public ActionResult Viewcharge()
        {
            int spid = int.Parse(Session["sp_id"].ToString());

            var s = (from n in dc.Charge_rate

                     where n.sp_id == spid
                     select n).ToList();
            ViewData["temp"] = s;
            return View();
        }

        public ActionResult UpdateCharge(int id)
        {
            var s = (from n in dc.Charge_rate

                     where n.charge_id == id
                     select n).FirstOrDefault();
            ViewData["temp"] = s;
            return View();
        }
        [HttpPost]
        public ActionResult UpdateCharge(FormCollection fc)
        {
            string amount = fc["amount"];
            int chargeid = int.Parse(fc["cid"].ToString());

            var s = (from n in dc.Charge_rate

                     where n.charge_id == chargeid
                     select n).FirstOrDefault();
            s.amount = decimal.Parse(amount.ToString());
            dc.SaveChanges();
            return RedirectToAction("Viewcharge");

        }

        public ActionResult ViewServiceInquiry()
        {
            int spid = int.Parse(Session["sp_id"].ToString());
            var s = (from n in dc.User_regis

                     from m in dc.user_request

                     where n.user_id == m.user_id && m.sp_id == spid && m.status == "Pending"
                     select new { n.fname, n.lname, n.user_id, m.userreq_id, n.mobile, n.email, n.address, m.status, m.description, m.date, m.token }).OrderByDescending(c => c.status).ToList();

            List<ServiceInquiry> list = new List<ServiceInquiry>();

            foreach (var item in s)
            {
                ServiceInquiry obj = new ServiceInquiry();
                obj.fname = item.fname;
                obj.lname = item.lname;
                obj.status = item.status;
                obj.token = item.token;
                obj.address = item.address;
                obj.date = item.date;
                obj.description = item.description;
                obj.mobile = item.mobile;
                obj.user_id = item.user_id;
                obj.userreq_id = item.userreq_id;

                list.Add(obj);
            }


            ViewData["temp"] = list;
            return View();
        }

        public ActionResult TodayInqury()
        {
            int spid = int.Parse(Session["sp_id"].ToString());

            var Todayinquiry = (from n in dc.User_regis

                                from m in dc.user_request

                                where n.user_id == m.user_id && m.sp_id == spid && m.status == "Pending" && (m.date.Value.Year == DateTime.Now.Year && m.date.Value.Month == DateTime.Now.Month && m.date.Value.Day == DateTime.Now.Day)
                                select new { n.fname, n.lname, n.user_id, m.userreq_id, n.mobile, n.email, n.address, m.status, m.description, m.date, m.token }).ToList();

            List<ServiceInquiry> list = new List<ServiceInquiry>();

            foreach (var item in Todayinquiry)
            {
                ServiceInquiry obj = new ServiceInquiry();
                obj.fname = item.fname;
                obj.lname = item.lname;
                obj.status = item.status;
                obj.token = item.token;
                obj.address = item.address;
                obj.date = item.date;
                obj.description = item.description;
                obj.mobile = item.mobile;
                obj.user_id = item.user_id;
                obj.userreq_id = item.userreq_id;

                list.Add(obj);
            }


            ViewData["temp"] = list;
            return View();
        }

        public ActionResult ViewInprocessInquiry()
        {
            int spid = int.Parse(Session["sp_id"].ToString());
            var s = (from n in dc.User_regis

                     from m in dc.user_request

                     where n.user_id == m.user_id && m.sp_id == spid && m.status == "In Process"
                     select new { n.fname, n.lname, m.processdate, n.user_id, m.enddate, m.userreq_id, n.mobile, n.email, n.address, m.status, m.description, m.date, m.token }).OrderByDescending(c => c.status).ToList();

            List<ServiceInquiry> list = new List<ServiceInquiry>();

            foreach (var item in s)
            {
                ServiceInquiry obj = new ServiceInquiry();
                obj.fname = item.fname;
                obj.lname = item.lname;
                obj.status = item.status;
                obj.token = item.token;
                obj.address = item.address;
                obj.date = item.date;
                obj.description = item.description;
                obj.mobile = item.mobile;
                obj.user_id = item.user_id;
                obj.userreq_id = item.userreq_id;
                obj.processdate = item.processdate;
                obj.enddate = item.enddate;

                list.Add(obj);
            }


            ViewData["temp"] = list;
            return View();

        }

        public ActionResult TotalInquiry()
        {
            int spid = int.Parse(Session["sp_id"].ToString());
            var s = (from n in dc.User_regis

                     from m in dc.user_request

                     where n.user_id == m.user_id && m.sp_id == spid
                     select new { n.fname, n.lname, m.processdate, n.user_id, m.enddate, m.userreq_id, n.mobile, n.email, n.address, m.status, m.description, m.date, m.token }).OrderByDescending(c => c.status).ToList();

            List<ServiceInquiry> list = new List<ServiceInquiry>();

            foreach (var item in s)
            {
                ServiceInquiry obj = new ServiceInquiry();
                obj.fname = item.fname;
                obj.lname = item.lname;
                obj.status = item.status;
                obj.token = item.token;
                obj.address = item.address;
                obj.date = item.date;
                obj.description = item.description;
                obj.mobile = item.mobile;
                obj.user_id = item.user_id;
                obj.userreq_id = item.userreq_id;
                obj.processdate = item.processdate;
                obj.enddate = item.enddate;

                list.Add(obj);
            }


            ViewData["temp"] = list;
            return View();

        }

        public ActionResult UpdateInquiryStatus(int id)
        {


            int spid = int.Parse(Session["sp_id"].ToString());
            var s = (from n in dc.Charge_rate
                     from m in dc.user_request
                     from k in dc.User_regis
                     where n.sp_id == spid && m.status == "In Process" && m.userreq_id == id && n.sp_id == m.sp_id && m.user_id == k.user_id
                     select new { n.amount, k.fname, m.token, k.lname, m.userreq_id }).FirstOrDefault();
            List<ServiceInquiry> list = new List<ServiceInquiry>();
            ServiceInquiry obj = new ServiceInquiry();
            obj.userreq_id = int.Parse(s.userreq_id.ToString());
            obj.amount = s.amount;
            obj.fname = s.fname;
            obj.lname = s.lname;
            obj.token = s.token;
            ViewData["temp"] = obj;
            return View();

        }
        [HttpPost]
        public ActionResult UpdateInquiryStatus(FormCollection fc)
        {
            int reqid = int.Parse(fc["reqid"].ToString());
            decimal amount = decimal.Parse(fc["charge"].ToString());
            string description = fc["pdesc"];
            decimal extracharge = decimal.Parse(fc["extraamount"].ToString());

            int token = int.Parse(fc["token"].ToString());
            int databasetoken = int.Parse(fc["dbtoken"].ToString());

            if (token == databasetoken)
            {
                Payment obj = new Payment();

                obj.date = DateTime.Now;
                obj.status = "Completed";
                obj.amount = amount;
                obj.userreq_id = reqid;
                obj.extra_amount = extracharge;
                obj.totamount = amount + extracharge;
                obj.description = description;
                obj.payment_mode = "COD";
                dc.Payments.Add(obj);
                dc.SaveChanges();

                var p = dc.user_request.Find(reqid);
                p.status = "Done";
                p.enddate = DateTime.Now;
                dc.SaveChanges();
                return RedirectToAction("ViewInprocessInquiry");

            }
            else
            {
                return RedirectToAction("ViewInprocessInquiry");


            }


        }
        public ActionResult Customer()
        {
            int spid = int.Parse(Session["sp_id"].ToString());

            var s = (from n in dc.Plan_Enroll
                     from l in dc.User_regis
                     from m in dc.Tbl_plan
                     where n.user_id == l.user_id && n.plan_id == m.plan_id && n.sp_id == spid
                     select new { n.plan_date, n.plan_status, n.planenroll_id, n.sp_id, n.planend_date, n.user_id, l.fname, m.plan_name }).ToList();

            List<plandisp> list = new List<plandisp>();

            foreach (var item in s)
            {
                plandisp obj = new plandisp();
                obj.plan_date = item.plan_date;
                obj.plan_status = item.plan_status;
                obj.planenroll_id = item.planenroll_id;
                obj.sp_id = item.sp_id;
                obj.planend_date = item.planend_date;
                obj.user_id = item.user_id;
                obj.plan_name = item.plan_name;
                obj.fname = item.fname;


                list.Add(obj);
            }

            var sp = list.GroupBy(test => test.user_id).Select(grp => grp.First()).ToList();
            ViewData["temp"] = sp;
            return View();
        }

        public ActionResult ViewAdminplan()
        {
            var p = dc.AdminSp_plan.ToList();
            ViewData["temp"] = p;
            return View();
        }

        public ActionResult PlanDetail(int id)
        {
            var p = dc.AdminSp_plan.Find(id);
            ViewData["temp"] = p;
            return View();
        }
        [HttpPost]
        public ActionResult PlanDetail(FormCollection fc)
        {
            int spid = int.Parse(Session["sp_id"].ToString());
            int otp = int.Parse(GenerateRandomNo().ToString());


            int subid = int.Parse(fc["pid"].ToString());

            string plan_description = fc["pdesc"];

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

            var p = (from n in dc.Sp_plan_Enroll where n.sub_id == subid && n.sp_id == spid select n).ToList().Count();
            if (p == 1)
            {
                ViewBag.msg = "!...you Already Enrolled this Plan";
                return RedirectToAction("ViewAdminplan");

            }
            else
            {

                Sp_plan_Enroll obj = new Sp_plan_Enroll();
                obj.sub_id = subid;
                obj.sp_id = spid;
                obj.startdate = DateTime.Now;
                obj.enddate = DateTime.Parse(enddate);
                obj.status = "Block";
                dc.Sp_plan_Enroll.Add(obj);
                dc.SaveChanges();
                ViewBag.msg = "Plan Successfully Enrolled..";

                var sp = (from n in dc.Sp_plan_Enroll where n.sp_id == spid select n).FirstOrDefault();
                sp.Otpnumber = otp;
                dc.SaveChanges();
                //insert enroll

                //sms to admin :otp send
                string Password = "hi";
                string Msg = "Enter Otp After Payment" + otp;

                string OPTINS = "CODE";
                string mob = "1234567890";

                string type = "3";
                string strUrl = "https://www.bulksmsgateway.in/sendmessage.php?user=hi&password=" + Password + "&message=" + Msg + "&sender=" + OPTINS + "&mobile=" + mob + "&type=" + 3;

                System.Net.WebRequest request = System.Net.WebRequest.Create(strUrl);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                Stream z = (Stream)response.GetResponseStream();
                StreamReader readStream = new StreamReader(z);
                string dataString = readStream.ReadToEnd();
                response.Close();
                z.Close();
                readStream.Close();
                //sms to Service provider:


                return RedirectToAction("AdminPayment");
            }

        }

        public ActionResult AdminPayment()
        {
            ViewBag.branch = "BranchV";
            ViewBag.acc = "xxxx1234";
            ViewBag.IFSC = "IFSC: Code BARB0VASADX";
            ViewBag.MICR = "MICR Code388012131";
            return View();
        }
        [HttpPost]
        public ActionResult AdminPayment(FormCollection fc)
        {
            int spid = int.Parse(Session["sp_id"].ToString());

            int otpno = int.Parse(fc["otp"].ToString());

            //update status
            var p1 = (from n in dc.Sp_plan_Enroll where n.sp_id == spid select n).FirstOrDefault();
            p1.status = "Approve";
            dc.SaveChanges();
            //
            var p = (from n in dc.Sp_plan_Enroll where n.sp_id == spid && n.Otpnumber == otpno select n).Count();
            if (p == 1)
            {
                Session["temp1"] = "1";
                return RedirectToAction("Dashboard", "Servicep");

            }
            else
            {
                return RedirectToAction("ViewAdminplan", "Servicep");
            }

        }
    }
}

public class Spmaintain
{

    public int planenroll_id { get; set; }
    public string fname { get; set; }
    public decimal installment { get; set; }
    public string userid { get; set; }

}
