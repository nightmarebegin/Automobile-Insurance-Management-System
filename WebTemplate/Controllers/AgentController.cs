using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using WebTemplate.Models;
using WebTemplate.Models.Model;

namespace WebTemplate.Controllers
{
    public class AgentController : Controller
    {
        IMSEntities1 db = new IMSEntities1();
        public bool SessionCheck()
        {
            if (Session["Id"] == null)
            {
                return false;
            }
            return true;

        }
        // GET: Agent
        public ActionResult Index()
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                List<USER> user = db.USERS.ToList();
                List<CustomerAgent> customerAgents = db.CustomerAgents.ToList();
                int id = (int)Session["Id"];
                var customerList = from u in user
                                   join c in customerAgents
                                   on u.Id equals c.UserId
                                   where c.AgentId == id
                                   select new getCustomerList
                                   {
                                       user = u,
                                       customerAgent = c
                                   };
                if (customerList == null)
                {
                    Session["Success"] = "No customers assigned yet";
                }
                return View(customerList);
            }
            catch(Exception e)
            {
                Session["Success"] = "Problem in getting customer list";
                return View("ManageProfile", "Agent");
            }
            
        }
        public ActionResult ManageProfile()
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                int id = (int)Session["Id"];
                var customerData = db.USERS.Where(x => x.Id == id).FirstOrDefault();
                return View(customerData);
            }
            catch (Exception e)
            {
                return View();
            }

        }

        [HttpPost]
        public ActionResult ManageProfile(USER user)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                int userid = (int)Session["Id"];
                var customer = db.USERS.Where(x => x.Email == user.Email && x.Id != userid).FirstOrDefault();
                if (customer == null)
                {
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    Session["Success"] = "Customer Details Updated Successfully";
                    return View();
                }
                Session["Success"] = "Email Id already exists";
                return View();

            }
            catch (Exception e)
            {
                ViewBag.Message = "Problem in updating the Agent details: " + e;
                return View();
            }

        }
        public ActionResult ManageComplaints()
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                int id = (int)Session["Id"];
                var complaintList = db.Complaints.Where(x => x.UserId == id).ToList();
                if (complaintList == null)
                {
                    Session["Success"] = "Complaint list is empty";
                }
                return View(complaintList);
            }
            catch (Exception e)
            {
                Session["Success"] = "Problem in getting complaints" + e;
                return RedirectToAction("ManageProfile", "Agent");
            }

        }
        public ActionResult AddComplaints()
        {
            if (SessionCheck() != true)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddComplaints(Complaint complaint)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                DateTime currentDatetime = DateTime.Now;
                complaint.CreatedDate = currentDatetime;
                complaint.UserId = (int)Session["Id"];
                db.Complaints.Add(complaint);
                db.SaveChanges();
                Session["Success"] = "Complaint Sent Successfully";
                return View();
            }
            catch (Exception e)
            {
                Session["Success"] = "Problem in submitting the complaint details: " + e;
                return View();
            }
        }

        public ActionResult transaction()
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                int id = (int)Session["Id"];
                List<PoliciesTransaction> transactions = db.PoliciesTransactions.ToList();
                List<CustomerAgent> customerAgents = db.CustomerAgents.ToList();
                var customerList = from u in transactions
                                   join c in customerAgents
                                   on u.UserId equals c.UserId
                                   where c.AgentId == id
                                   select new getTransactionList
                                   {
                                       tran = u,
                                       customerAgent = c
                                   };
                if (customerList == null)
                {
                    Session["Success"] = "Customer transaction are empty or No customer assigned yet";
                }
                return View(customerList);
            }
            catch(Exception e)
            {
                Session["Success"] = "Problem in getting transaction list" + e;
                return RedirectToAction("ManageProfile", "Agent");
            }
            
        }

        public ActionResult viewDetails(int userid)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                var customerDetails = db.USERS.Where(x => x.Id == userid).FirstOrDefault();
                return View(customerDetails);
            }
            catch(Exception e)
            {
                Session["Success"] = "Problem in getting customer details" + e;
                return RedirectToAction("Index", "Agent");
            }
            
        }
        public ActionResult sendEmailCustomer(string Email)
        {
            if (SessionCheck() != true)
            {
                return RedirectToAction("Index", "Login");
            }
            Session["email"] = Email;
            return View();
        }

        [HttpPost]
        public ActionResult sendEmailCustomer(Complaint complaint)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                if (SendEmail((string)Session["Email"], complaint.Message))
                {
                    Session["Success"] = "Mail sent to customer successfully";
                    return RedirectToAction("Index", "Agent");
                }
                Session["Success"] = "Mail not sent to customer";
                return RedirectToAction("Index", "Agent");
            }
            catch(Exception e)
            {
                Session["Success"] = "Mail not sent to customer";
                return RedirectToAction("Index", "Agent");
            }
            
        }
        public bool SendEmail(string EmailId, string bo)
        {
            var fromEmail = new MailAddress("ims_insurance@outlook.com", "IMS");
            var toEmail = new MailAddress(EmailId);
            var fromEmailPassword = "1234@4321ims"; // Replace with actual password

            string subject = "";
            string body = "";

            subject = "AIMS:Agent Contact";
            body = ("<br/><br/>Hii hope you are doing well" +
                    "I am your agent and want to say something to you.The message is: " + bo +
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