using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebTemplate.Models;
using WebTemplate.Models.Model;

namespace WebTemplate.Controllers
{
    public class LoginController : Controller
    { 

        IMSEntities1 db=new IMSEntities1();
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(USER user)
        {
            try
            {
                USER loginData = new USER();
                loginData.Email = user.Email;
                loginData.Password = user.Password;

                var data = db.USERS.Where(x => x.Email.Equals(loginData.Email) && x.Password.Equals(loginData.Password)).FirstOrDefault();
                if (data != null)
                {
                    if (data.RoleId == 1)
                    {
                        Session["Id"] = data.Id;
                        Session["Name"] = data.Name;
                        return RedirectToAction("getPolicy", "Customer");


                    }
                    else if (data.RoleId == 2)
                    {
                        Session["Id"] = data.Id;
                        Session["Name"] = data.Name;
                        return RedirectToAction("Index", "Agent");
                    }
                    else
                    {
                        Session["Id"] = data.Id;
                        Session["Name"] = data.Name;
                        return RedirectToAction("Index", "Admin");
                    }
                }
                Session["Success"] = "Wrong Login Credentials";
                return View();
            }
            catch(Exception e)
            {
                Session["Success"]="Exception occured" +e;
                return View();
            }
            
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(USER user)
        {
            try
            {
                var customer = db.USERS.Where(x => x.Email == user.Email).FirstOrDefault();
                if (customer == null)
                {
                    USER customerData = new USER();
                    customerData.RoleId = 1;
                    customerData.Name = user.Name;
                    customerData.Email = user.Email;
                    customerData.Password = user.Password;
                    customerData.PhoneNumber = user.PhoneNumber;
                    customerData.Address = user.Address;
                    customerData.Gender = user.Gender;
                    /*if (ModelState.IsValid == true)
                    {*/
                    Random r = new Random();
                    int otp = r.Next(100000, 999999);
                    if (SendEmail(customerData.Email, otp))
                    {
                        Session["otp"] = otp;
                        Session["user"] = customerData;
                        return RedirectToAction("Verify");
                    }

                    return View();
                }
                Session["Success"] = "Email Id already exits";
                return View();
                /*}
                return View();*/
            }
            catch (Exception e)
            {
                Session["Success"] = "Exception occured"+e;
                return View();
            }  
        }
        public bool SendEmail(string EmailId,int otp)
        {
            var fromEmail = new MailAddress("", "AIC");
            var toEmail = new MailAddress(EmailId);
            var fromEmailPassword = ""; // Replace with actual password

            string subject = "";
            string body = "";

            subject = "AIC:Email Verification";
            body = ("<p><h1>Verify your email address</h1></ p > " +
                "<br/><br/><br/><br/>" +
                "To finish setting up your Insurance account, we just need to make sure this email address is yours." +
                " < br />< br />< br />< br /> " +
                    "Your OTP is " + otp +
                    " <br/><br/>" +
                    "If you didn't request this code, you can safely ignore this email. Someone else might have typed your email address by mistake."
                    +
                    "<br><br><br>" +
                    "Thanks," +
                    "<br>"
                    +
                    "The Vivek Pvt Ltd account team"
                    );



           


            var smtp = new SmtpClient
            {
                Host = "smtp.live.com",
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword),
                EnableSsl = true
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
            return true;
            
        }

        public ActionResult Verify()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Verify(Verify verify)
        {
            try
            {
                USER customerData = new USER();
                DateTime currentDate = DateTime.Now;
                customerData = (USER)Session["user"];
                customerData.CreatedDate = currentDate;
                if (verify.otp == Convert.ToInt32(Session["otp"]))
                {
                    db.USERS.Add(customerData);
                    db.SaveChanges();
                    ViewBag.Message = "Customer Created Successfully";
                    return View("Index");
                }
                Session["Success"] = "Please enter correct OTP";
                return View();
            }
            catch(Exception e)
            {
                Session["Success"] = "Exception occured" +e;
                return View();
            }
            
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index","login");
        }

        public ActionResult forgotPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult forgotPassword(forgotPassword forgotPassword)
        {
            try
            {
                string email = (string)Session["verify"];

                var user = db.USERS.Where(x => x.Email == email).FirstOrDefault();

                user.Password = forgotPassword.password;
                db.SaveChanges();
                Session["Success"] = "Password changed successfully";
                return RedirectToAction("Index", "login");
            }
            catch(Exception e)
            {
                Session["Success"] = "Problem in changing password"+e;
                return RedirectToAction("Index", "login");
            }
            
                
            
        }
        public ActionResult verifyOtp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult verifyOtp(Verify verify)
        {
            try
            {
                
                if (verify.otp == Convert.ToInt32(Session["otp"]))
                {

                    Session["Success"] = "OTP verified successfully.";
                    return RedirectToAction("forgotPassword","login");
                }
                Session["Success"] = "Please enter correct OTP.";
                return View();
            }
            catch (Exception Message)
            {
                ViewBag.Message = "Exception occured";
                return View();
            }

        }
        public ActionResult VerifyByEmail()
        {
            return View();
        }
        [HttpPost]
        public ActionResult VerifyByEmail(verifyByEmail verify)
        {
            try
            {

                /*if (ModelState.IsValid == true)
                {*/
                var user = db.USERS.Where(x => x.Email == verify.Email).FirstOrDefault();
                if (user != null)
                {
                    Random r = new Random();
                    int otp = r.Next(100000, 999999);
                    if (SendEmailForForgotPassword(verify.Email, otp))
                    {
                        Session["otp"] = otp;
                        Session["verify"] = verify.Email;
                        Session["Success"] = "An OTP sent your email.";
                        return RedirectToAction("verifyOtp", "login");
                    }
                }
                Session["Success"] = "Email-ID does not exist";
                return View();
                /*}
                return View();*/
            }
            catch (Exception e)
            {
                Session["Success"] = "Exception occured";
                return View();
            }
        }
        public bool SendEmailForForgotPassword(string EmailId, int otp)
        {
            var fromEmail = new MailAddress("", "AIC");
            var toEmail = new MailAddress(EmailId);
            var fromEmailPassword = ""; // Replace with actual password

            string subject = "";
            string body = "";

            subject = "AIC:Email Verification";
            body = ("<br/><br/>We are excited to tell you that your Insurance account needs email verifiction for Password Reset" +
                    "Your OTP is " + otp +
                    " <br/><br/>");




            var smtp = new SmtpClient
            {
                Host = "smtp.live.com",
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword),
                EnableSsl = true
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
            return true;

        }
    }
}