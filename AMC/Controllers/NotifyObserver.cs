using AMC.EDM;
using System.Net.Mail;
class NotifyObserver : Observer
  {
    private User_regis user;
    private string observerState;
    private PlanSubject subject;

    // Constructor
    public NotifyObserver(PlanSubject subject, User_regis user)
    {
      this.subject = subject;
      this.user = user;
    }

    public override void SendEmail()
    {
      observerState = "Opted";
      //emailId = user.email;
      MailMessage mail = new MailMessage();
      SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
      mail.From = new MailAddress("backup200397@gmail.com");
      mail.To.Add(user.email);
      mail.Subject = "NOTIFY: " + subject.SubjectState;
      string body = "Hi Thank You For being with us. I just want to let you know that a new plan has been added." + user.email.ToString() + "\n";
      body += "";
      mail.Body = body;
      SmtpServer.Port = 587;
      SmtpServer.Credentials = new System.Net.NetworkCredential("backup200397@gmail.com", "utilitybackup");
      SmtpServer.EnableSsl = true;
      SmtpServer.Send(mail);
    }

    // Property
    public PlanSubject Subject
    {
      get { return subject; }
      set { subject = value; }
    }
  }