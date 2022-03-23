using AMC.EDM;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AMC.Controllers
{
    #region Bot Properties
    public class BotModel
    {
        public string question { get; set; }
    }
    public partial class BotReply
    {
        [JsonProperty("answers")]
        public Answer[] Answers { get; set; }
    }

    public partial class Answer
    {
        [JsonProperty("questions")]
        public string[] Questions { get; set; }

        [JsonProperty("answer")]
        public string AnswerAnswer { get; set; }

    }
    #endregion

    public class HomeController : Controller
    {
        DBServiceProviderEntities dc = new DBServiceProviderEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Homepage()
        {
            var p = dc.ServiceproviderTypes.ToList();
            ViewData["temp"] = p;
            return View();
        }
        public ActionResult Aboutus()
        {
            return View();
        }
        public ActionResult Askus()
        {
            return View();
        }
        public ActionResult Contactus()
        {
            return View();
        }
        public ActionResult Services()
        {
            return View();
        }
        public ActionResult Gallery()
        {
            return View();

        }
        public ActionResult ViewServices()
        {
            var p = dc.ServiceproviderTypes.ToList();
            ViewData["temp"] = p;
            return View();
        }
        public JsonResult getdata(int cid)
        {
            var p = (from n in dc.City_master
                     from l in dc.Area_master
                     where n.city_id == l.city_id && n.city_id == cid
                     select new { l.area_id, l.area_name }).ToList();
            return Json(p, JsonRequestBehavior.AllowGet);

        }

        public ActionResult AdminLogin()
        {
            return View();
        }
        [HttpPost]

        public ActionResult AdminLogin(FormCollection fc)
        {
            string username = fc["username"].ToString();
            string password = fc["password"].ToString();
            var login = (from l in dc.tblAdminLogins where l.UserName == username && l.Password == password select l).Count();

            if (login == 1)
            {
                return RedirectToAction("Dashboard", "Admin");
            }
            else
            {
                ViewBag.msg = "Invalid User Name";
                return View();

            }
        }

        public ActionResult UserLogin()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UserLogin(FormCollection fc)
        {
            string username = fc["username"].ToString();
            string password = fc["password"].ToString();

            var p = (from n in dc.User_regis where n.email == username && n.password == password && n.status == "Approve" select n);
            if (p.Count() == 1)
            {
                var user_id = p.FirstOrDefault().user_id;

                Session["user_id"] = user_id;
                Session["user_name"] = p.FirstOrDefault().fname;




                return RedirectToAction("Dashboard", "User");

            }
            else
            {

                ViewBag.msg = "Invalid User Name";
                return View();


            }

        }
        public ActionResult SpLogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SpLogin(FormCollection fc)
        {
            string username = fc["username"].ToString();
            string password = fc["password"].ToString();

            var p = (from n in dc.Serviceprovider_regis where n.sp_email == username && n.password == password && n.sp_status == "Approve" select n);
            if (p.Count() == 1)
            {
                var sp_id = p.FirstOrDefault().sp_id;
                Session["sp_id"] = sp_id;
                Session["user_name"] = p.FirstOrDefault().sp_fname;

                var cnt = dc.Sp_plan_Enroll.Where(c => c.sp_id == sp_id).Count();

                if (cnt == 1)
                {
                    var sts = dc.Sp_plan_Enroll.Where(c => c.sp_id == sp_id).FirstOrDefault().status;
                    if (sts == "Approve")
                    {
                        var dt = dc.Sp_plan_Enroll.Where(c => c.sp_id == sp_id).FirstOrDefault().enddate;
                        if (DateTime.Now < dt)
                        {
                            Session["temp1"] = "1";
                            return RedirectToAction("Dashboard", "Servicep");

                        }
                        else
                        {
                            Session["temp1"] = "0";

                            return RedirectToAction("Dashboard", "Servicep");
                        }
                    }
                    else
                    {
                        return RedirectToAction("Dashboard", "Servicep");
                    }

                }
                else
                {
                    Session["temp1"] = "0";
                    return RedirectToAction("Dashboard", "Servicep");
                }


            }
            else
            {
                ViewBag.msg = "Invalid User Name";
                return View();


            }

        }
        public ActionResult SpRegistration()
        {
            var p = dc.ServiceproviderTypes.ToList();
            ViewData["sp_type"] = p;

            return View();
        }

        [HttpPost]
        public ActionResult SpRegistration(FormCollection fc, HttpPostedFileBase file)
        {
            if (file != null)
            {
                string filename = System.IO.Path.GetFileName(file.FileName);
                string fullpath = System.IO.Path.Combine(Server.MapPath("~/logo/"), filename);
                file.SaveAs(fullpath);

                string dbpath = "/logo/" + filename;


            }

            string firstname = fc["firstname"].ToString();
            string lastname = fc["lastname"].ToString();
            string email = fc["email"].ToString();
            string spid = fc["spid"];
            string password = fc["password"].ToString();
            string mobno = fc["mobno"];
            string gender = fc["gender"].ToString();
            string address = fc["address"].ToString();

            Serviceprovider_regis obj = new Serviceprovider_regis();

            obj.sp_fname = firstname;
            obj.sp_lname = lastname;
            obj.sp_email = email;
            obj.sptype_id = int.Parse(spid.ToString());
            obj.password = password;
            obj.sp_mobile = long.Parse(mobno);
            obj.sp_logo = "/logo/" + file.FileName;
            obj.spregis_date = DateTime.Now;
            obj.sp_status = "Pending";
            obj.address = address;
            obj.sp_gender = gender;
            dc.Serviceprovider_regis.Add(obj);
            dc.SaveChanges();

            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("backup200397@gmail.com");
            mail.To.Add(email);
            mail.Subject = "WELCOME TO UTILITY PROVIDER";
            string body = "Hi Thank You For Register on our Utility Provider. " + firstname.ToString() + "\n";
            body += "Your User Name is:" + email.ToString() + "\n";
            body += "Your Password is:" + password.ToString() + "\n";
            body += "";
            mail.Body = body;
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("backup200397@gmail.com", "utilitybackup");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);



            return RedirectToAction("SpLogin");

        }

        public ActionResult UserRegistration()
        {
            var p = dc.City_master.ToList();
            ViewData["city"] = p;
            var p1 = dc.Area_master.ToList();

            ViewData["area"] = p1;
            return View();
        }
        [HttpPost]
        public ActionResult UserRegistration(FormCollection fc, HttpPostedFileBase file)
        {
            if (file != null)
            {
                string filename = System.IO.Path.GetFileName(file.FileName);
                string fullpath = System.IO.Path.Combine(Server.MapPath("~/user_image/"), filename);
                file.SaveAs(fullpath);

                string dbpath = "/user_image/" + filename;
            }

            string firstname = fc["firstname"].ToString();
            string lastname = fc["lastname"].ToString();
            string email = fc["email"].ToString();
            string city_id = fc["city"];
            string area_id = fc["area"];
            string password = fc["password"].ToString();
            string mobno = fc["mobno"];
            string gender = fc["gender"].ToString();
            string address = fc["address"].ToString();
            string path = "/user_image/" + file.FileName;

            User_regis obj = new User_regis();
            obj.fname = firstname;
            obj.lname = lastname;
            obj.email = email;
            obj.city_id = int.Parse(city_id.ToString());
            obj.area_id = int.Parse(area_id.ToString());
            obj.password = password;
            obj.mobile = long.Parse(mobno);
            obj.user_img = path;
            obj.date = DateTime.Now;
            obj.status = "Approve";
            obj.address = address;
            obj.gender = gender;
            dc.User_regis.Add(obj);
            dc.SaveChanges();
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("backup200397@gmail.com");
            mail.To.Add(email);
            mail.Subject = "WELCOME TO UTILITY PROVIDER";
            string body = "Hi Thank You for the registration on our Utility Provider. " + email.ToString() + "\n";
            body += "Your User Name is:" + email.ToString() + "\n";
            body += "Your Password is:" + password.ToString() + "\n";
            body += "";
            mail.Body = body;
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("backup200397@gmail.com", "utilitybackup");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
            return RedirectToAction("UserLogin");

        }

        public ActionResult Forgotpwd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Forgotpwd(FormCollection fc)
        {
            string email1 = fc["email1"].ToString();
            var p = dc.User_regis.Where(c => c.email == email1).FirstOrDefault().password;
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("backup200397@gmail.com");
            mail.To.Add(email1);
            mail.Subject = "Your forgotten password";
            string body = "Password for your account " + email1.ToString() + "\n";
            body += "Your User Name is: " + email1.ToString() + "\n";
            body += "Your Password is: " + p.ToString() + "\n";
            body += "";
            mail.Body = body;
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("backup200397@gmail.com", "utilitybackup");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
            return RedirectToAction("UserLogin");
        }

        public ActionResult Spforgotpwd()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Spforgotpwd(FormCollection fc)
        {
            string email1 = fc["email1"].ToString();
            var p = dc.Serviceprovider_regis.Where(c => c.sp_email == email1).FirstOrDefault().password;
            MailMessage mail = new MailMessage();
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            mail.From = new MailAddress("backup200397@gmail.com");
            mail.To.Add(email1);
            mail.Subject = "Your forgotten password";
            string body = "Password for your account " + email1.ToString() + "\n";
            body += "Your User Name is: " + email1.ToString() + "\n";
            body += "Your Password is: " + p.ToString() + "\n";
            body += "";
            mail.Body = body;
            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("backup200397@gmail.com", "utilitybackup");
            SmtpServer.EnableSsl = true;
            SmtpServer.Send(mail);
            return RedirectToAction("SpLogin");
        }

        #region Bot API
        [HttpPost]
        public async Task<string> GetBotReplyAsync(string question)
        {
            if (question.Equals("") || string.IsNullOrEmpty(question))
                return "Please enter something";

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            string URL = "https://qna-bot-b36b.azurewebsites.net/qnamaker/knowledgebases/c5f1eda0-7427-473b-8aa7-1d432ff28a29/generateAnswer";

            BotModel botQuestion = new BotModel();
            botQuestion.question = question;

            var clientProducer = new HttpClient();

            var json = JsonConvert.SerializeObject(botQuestion);
            var data = new StringContent(json, Encoding.UTF8, "application/json");

            clientProducer.DefaultRequestHeaders.Add("Authorization", "EndpointKey c356c30e-00d3-4dc2-be35-3937dfee0df5");

            HttpResponseMessage responseProducer = await clientProducer.PostAsync(URL, data);
            string strResponseForInstance = "";
            if (responseProducer != null)
            {
                strResponseForInstance = responseProducer.Content.ReadAsStringAsync().Result;
            }
            var user = JsonConvert.DeserializeObject<BotReply>(strResponseForInstance);

            return user.Answers[0].AnswerAnswer;
        }

        #endregion
    }
}
