using AMC.EDM;
using AMC.Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
namespace AMC.Controllers
{
    public class AdminController : Controller
    {

        DBServiceProviderEntities dc = new DBServiceProviderEntities();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            var totuser = dc.User_regis.Count();
            ViewBag.cnt = totuser;

            var totsp = dc.Serviceprovider_regis.Count();
            ViewBag.cntsp = totsp;

            var totservice = dc.ServiceproviderTypes.Count();
            ViewBag.cntservice = totservice;

            var totcomp = dc.User_complaint.Count();
            ViewBag.cntcomplaint = totcomp;

            var totpendingcomp = dc.User_complaint.Where(c => c.status == "Pending").Count();
            ViewBag.cntpcomp = totpendingcomp;

            var Totalinquiry = (from n in dc.User_regis
                                from k in dc.Serviceprovider_regis
                                from m in dc.user_request

                                where n.user_id == m.user_id && k.sp_id == m.sp_id
                                select new { n.fname, n.lname, k.sp_fname, k.sp_lname, n.user_id, m.userreq_id, n.mobile, n.email, n.address, m.status, m.description, m.date, m.token }).Count();
            ViewBag.TotInq = Totalinquiry;


            return View();
        }
        public ActionResult AddCity()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddCity(FormCollection fc)
        {

            string cityname = fc["cityname"].ToLower();
            var p = (from n in dc.City_master where n.city_name == cityname select n).ToList().Count();

            if (p == 1)
            {
                ViewBag.msg = "city already Exist";
                return View();

            }
            else
            {
                City_master obj = new City_master();
                obj.city_name = cityname;
                dc.City_master.Add(obj);
                dc.SaveChanges();
                ViewBag.msg = "Record Successfully Inserted..";
                return View();
            }
            //


        }
        public ActionResult ViewCity()
        {
            var p = dc.City_master.ToList();
            ViewData["temp"] = p;
            return View();
        }

        public ActionResult DeleteCity(int id)
        {
            var p = (from n in dc.City_master where n.city_id == id select n).FirstOrDefault();
            dc.City_master.Remove(p);
            dc.SaveChanges();

            return RedirectToAction("ViewCity");
        }

        public ActionResult AddArea()
        {
            var p = dc.City_master.ToList();
            ViewData["city"] = p;

            return View();
        }

        [HttpPost]
        public ActionResult AddArea(FormCollection fc)
        {
            var p = dc.City_master.ToList();
            ViewData["city"] = p;
            string areaname = fc["areaname"].ToString().ToLower();
            int city_id = int.Parse(fc["cityname"].ToString());

            var p1 = (from n in dc.Area_master where n.city_id == city_id && n.area_name == areaname select n).ToList().Count();

            if (p1 == 1)
            {
                ViewBag.msg = "Area already Exist";
                return View();

            }
            else
            {
                Area_master obj = new Area_master();
                obj.area_name = areaname;
                obj.city_id = int.Parse(city_id.ToString());

                dc.Area_master.Add(obj);
                dc.SaveChanges();

                ViewBag.msg = "Record Successfully Inserted..";
                return View();
            }


        }

        public ActionResult ViewArea()
        {
            var p = dc.Area_master.ToList();
            ViewData["temp"] = p;
            return View();
        }

        public ActionResult ViewSp()
        {
            var s = (from n in dc.Serviceprovider_regis
                     from l in dc.ServiceproviderTypes


                     where n.sptype_id == l.sptype_id
                     select new { n.sp_lname, n.sp_fname, n.sp_email, n.sp_id, n.address, n.sp_logo, n.sp_status, n.spregis_date, n.sp_mobile, l.sptype_name }).ToList();
            List<Viewsp> list = new List<Viewsp>();

            foreach (var item in s)
            {
                Viewsp obj = new Viewsp();
                obj.sp_fname = item.sp_fname;
                obj.sp_lname = item.sp_lname;
                obj.sp_logo = item.sp_logo;
                obj.sp_mobile = item.sp_mobile;
                obj.sp_status = item.sp_status;
                obj.sp_email = item.sp_email;
                obj.sptype_name = item.sptype_name;
                obj.address = item.address;
                obj.spregis_date = item.spregis_date;
                obj.sp_id = item.sp_id;
                list.Add(obj);
            }


            ViewData["temp"] = list;
            return View();
        }
        public ActionResult Approve(int id)
        {
            var p = dc.Serviceprovider_regis.Find(id);
            p.sp_status = "Approve";
            dc.SaveChanges();
            return RedirectToAction("ViewSp");

        }
        public ActionResult Block(int id)
        {
            var p = dc.Serviceprovider_regis.Find(id);
            p.sp_status = "Block";
            dc.SaveChanges();
            return RedirectToAction("ViewSp");

        }

        public ActionResult ViewUser()
        {
            var p = dc.User_regis.OrderByDescending(c => c.status).ToList();
            ViewData["temp"] = p;
            return View();

        }
        public ActionResult ApproveUser(int id)
        {
            var p = dc.User_regis.Find(id);
            p.status = "Approve";
            dc.SaveChanges();
            return RedirectToAction("ViewUser");
        }
        public ActionResult BlockUser(int id)
        {
            var p = dc.User_regis.Find(id);
            p.status = "Block";
            dc.SaveChanges();
            return RedirectToAction("ViewUser");
        }

        public ActionResult AddSpType()
        {
            return View();

        }
        [HttpPost]
        public ActionResult AddSpType(FormCollection fc)
        {

            string sptype = fc["sptype"].ToLower();
            var p1 = (from n in dc.ServiceproviderTypes where n.sptype_name == sptype select n).ToList().Count();

            if (p1 == 1)
            {
                ViewBag.msg = " already Exist";
                return View();

            }

            ServiceproviderType obj = new ServiceproviderType();
            obj.sptype_name = sptype;
            dc.ServiceproviderTypes.Add(obj);
            dc.SaveChanges();
            ViewBag.msg = "Record Successfully Inserted";
            return View();
        }
        public ActionResult ViewSptype()
        {
            var p = dc.ServiceproviderTypes.ToList();
            ViewData["temp"] = p;
            return View();
        }
        public ActionResult DeleteSpType(int id)
        {
            var p = (from n in dc.ServiceproviderTypes where n.sptype_id == id select n).FirstOrDefault();
            dc.ServiceproviderTypes.Remove(p);
            dc.SaveChanges();
            return RedirectToAction("ViewSptype");
        }

        public ActionResult UpdateSpType(int id)
        {
            var p = (from n in dc.ServiceproviderTypes where n.sptype_id == id select n).FirstOrDefault();
            ViewData["temp"] = p;
            return View();
        }
        [HttpPost]
        public ActionResult UpdateSpType(FormCollection fc)
        {

            string sptype = fc["sptypeid"];
            string sptypename = fc["sptype"].ToString();
            int sptypee = int.Parse(sptype.ToString());
            var p = (from n in dc.ServiceproviderTypes where n.sptype_id == sptypee select n).FirstOrDefault();
            p.sptype_name = sptypename;


            dc.SaveChanges();
            ViewBag.msg = "Record Updated";
            return RedirectToAction("ViewSptype");
        }
        public ActionResult ViewPlanComplaint()
        {


            var s = (from n in dc.User_complaint
                     from l in dc.User_regis
                     from m in dc.Tbl_plan

                     where n.user_id == l.user_id && n.plan_id == m.plan_id
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

        public ActionResult ViewPendingComplaint()
        {
            var s = (from n in dc.User_complaint
                     from l in dc.User_regis
                     from m in dc.Tbl_plan

                     where n.user_id == l.user_id && n.plan_id == m.plan_id && n.status == "Pending"
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

        public ActionResult TotalInquiry()
        {
            var Totalinquiry = (from n in dc.User_regis
                                from k in dc.Serviceprovider_regis
                                from m in dc.user_request

                                where n.user_id == m.user_id && k.sp_id == m.sp_id
                                select new { n.fname, n.lname, k.sp_id, k.sp_fname, k.sp_lname, n.user_id, m.userreq_id, n.mobile, n.email, n.address, m.status, m.description, m.date, m.processdate, m.enddate }).ToList();

            List<ServiceInquiry> list = new List<ServiceInquiry>();

            foreach (var item in Totalinquiry)
            {
                ServiceInquiry obj = new ServiceInquiry();
                obj.fname = item.fname;
                obj.lname = item.lname;
                obj.sp_lname = item.sp_lname;
                obj.sp_fname = item.sp_fname;
                obj.status = item.status;
                obj.sp_id = item.sp_id;
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

        public ActionResult ViewServiceProvider(int id)
        {
            var s = (from n in dc.Serviceprovider_regis
                     from l in dc.ServiceproviderTypes


                     where n.sptype_id == l.sptype_id && n.sp_id == id
                     select new { n.sp_lname, n.sp_fname, n.sp_email, n.sp_id, n.address, n.sp_logo, n.sp_status, n.spregis_date, n.sp_mobile, l.sptype_name }).ToList();
            List<Viewsp> list = new List<Viewsp>();

            foreach (var item in s)
            {
                Viewsp obj = new Viewsp();
                obj.sp_fname = item.sp_fname;
                obj.sp_lname = item.sp_lname;
                obj.sp_logo = item.sp_logo;
                obj.sp_mobile = item.sp_mobile;
                obj.sp_status = item.sp_status;
                obj.sp_email = item.sp_email;
                obj.sptype_name = item.sptype_name;
                obj.address = item.address;
                obj.spregis_date = item.spregis_date;
                obj.sp_id = item.sp_id;
                list.Add(obj);
            }


            ViewData["temp"] = list;
            return View();
        }

        public ActionResult Spplan()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Spplan(FormCollection fc)
        {
            string name = fc["pname"].ToString().ToLower();
            string description = fc["pdesc"].ToString();
            decimal charge = decimal.Parse(fc["charge"].ToString());
            string duration = fc["duration"].ToString();

            var p1 = (from n in dc.AdminSp_plan where n.sub_name == name select n).ToList().Count();

            if (p1 == 1)
            {
                ViewBag.msg = "already Exist";
                return View();

            }
            else
            {
                AdminSp_plan obj = new AdminSp_plan();
                obj.sub_name = name;
                obj.amount = charge;
                obj.description = description;
                obj.duration = duration;
                obj.status = "Running";
                dc.AdminSp_plan.Add(obj);
                dc.SaveChanges();
                ViewBag.msg = "Successfully Added..";

                return View();
            }


        }

        public ActionResult ViewSpplan()
        {

            var p = (from n in dc.AdminSp_plan select n).ToList();

            ViewData["temp"] = p;
            return View();
        }
        public ActionResult DeletePlan(int id)
        {
            var p = (from n in dc.AdminSp_plan where n.sub_id == id select n).FirstOrDefault();
            dc.AdminSp_plan.Remove(p);
            dc.SaveChanges();
            return RedirectToAction("ViewSpplan");
        }
        public ActionResult Updateplan(int id)
        {
            var p1 = (from n in dc.AdminSp_plan where n.sub_id == id select n).FirstOrDefault();
            ViewData["temp"] = p1;
            return View();
        }

        [HttpPost]
        public ActionResult Updateplan(FormCollection fc)
        {



            int sub_id = int.Parse(fc["subid"].ToString());
            string name = fc["pname"].ToString();
            string description = fc["pdesc"].ToString();
            decimal charge = decimal.Parse(fc["charge"].ToString());
            string duration = fc["duration"].ToString();
            var p = (from n in dc.AdminSp_plan where n.sub_id == sub_id select n).FirstOrDefault();

            p.sub_name = name;
            p.amount = charge;
            p.description = description;
            p.duration = duration;


            dc.SaveChanges();
            ViewBag.msg = "Successfully Added..";

            return RedirectToAction("ViewSpplan");
        }

        public ActionResult ClosedPlan(int id)
        {
            var p = dc.AdminSp_plan.Find(id);
            p.status = "Closed";
            dc.SaveChanges();
            return RedirectToAction("ViewSpplan");
        }
        public ActionResult Running(int id)
        {
            var p = dc.AdminSp_plan.Find(id);
            p.status = "Running";
            dc.SaveChanges();
            return RedirectToAction("ViewSpplan");
        }

        public ActionResult UserDetail(int id)
        {
            var p = dc.User_regis.Find(id);
            ViewData["Temp"] = p;
            return View();
        }

    }
}
