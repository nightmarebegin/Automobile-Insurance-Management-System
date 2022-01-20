using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTemplate.Models;

namespace WebTemplate.Controllers
{
    public class WelcomeController : Controller
    {
        IMSEntities1 db = new IMSEntities1();

        // GET: Welcome
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(Feedback feedback)
        {
            try
            {
                DateTime currentDate = DateTime.Now;
                feedback.CreatedDate = currentDate;
                db.Feedbacks.Add(feedback);
                db.SaveChanges();
                Session["Success"] = "Feedback inserted successfully";
                return View();
            }
            catch(Exception e)
            {
                Session["Success"] = "Feedback not sent" +e;
                return View();
            }
            
        }
    }
}