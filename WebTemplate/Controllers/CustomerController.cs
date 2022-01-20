using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTemplate.Models;
using WebTemplate.Models.Model;

namespace WebTemplate.Controllers
{
    public class CustomerController : Controller
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
        // GET: Customer
        public ActionResult Index()
        {
            if (SessionCheck() != true)
            {
                return RedirectToAction("Index", "Login");
            }
            var categoryList = db.AutomobileCategories.ToList();
            SelectList list = new SelectList(categoryList,"Id","Name");
            ViewBag.categoryListItem = list;
            return View();
            
        }
        [HttpPost]
        public ActionResult Index(AutomobileCategory automobileCategory)
        {
            if (SessionCheck() != true)
            {
                return RedirectToAction("Index", "Login");
            }
            int id = automobileCategory.Id;
            var data = db.Policies.Where(x => x.CategoryId == id).ToList();
            return View("getPolicy",data);
        }

        public ActionResult getPolicy()
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                var data = db.Policies.ToList();
                return View(data);
            }
            catch(Exception e)
            {
                Session["Success"] = "Problem in getting policies"+e;
                return View();
            }
            
            
        }

        public ActionResult getPolicyDetails(int id)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                var policyData = db.Policies.Where(x => x.Id == id).FirstOrDefault();
                return View(policyData);
            }
            catch(Exception e)
            {
                Session["Success"] = "Problem in getting policies" + e;
                return RedirectToAction("getPolicy","Customer");
            }
            
        }
        [HttpPost]
        public ActionResult getPolicyDetails(Policy policy)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                int userid= (int)Session["Id"];
                var userpolicydetails = db.BuyPolicies.Where(x => x.PolicyId == policy.Id && x.UserId == userid).FirstOrDefault();
                if (userpolicydetails == null)
                {
                    BuyPolicy buyPolicy = new BuyPolicy();
                    buyPolicy.UserId = (int)Session["Id"];
                    buyPolicy.PolicyId = policy.Id;
                    buyPolicy.PolicyCategoryId = policy.CategoryId;
                    buyPolicy.AmountPaid = policy.PremiumAmount;
                    DateTime currentDate = DateTime.Now;
                    buyPolicy.CreatedDate = currentDate;
                    Session["buypolicydetails"] = buyPolicy;
                    /*db.BuyPolicies.Add(buyPolicy);
                    db.SaveChanges();
                    ViewBag.Message = "Policy buyed successfully";*/
                    return RedirectToAction("paymentDetails", "Customer");
                }
                Session["Success"] = "Policy is already buyed by you";
                return View();
            }
            catch(Exception e)
            {
                Session["Success"] = "Problem in buying the policy";
                return View();
            }
            
        }

        public ActionResult paymentDetails()
        {
            if (SessionCheck() != true)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        [HttpPost]
        public ActionResult paymentDetails( PaymentDetail paymentDetail)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }

                BuyPolicy buyPolicy = new BuyPolicy();
                buyPolicy = (BuyPolicy)Session["buypolicydetails"];
                paymentDetail.UserId = (int)buyPolicy.UserId;
                paymentDetail.PolicyId = (int)buyPolicy.PolicyId;
                DateTime currentdatetime = DateTime.Now;
                paymentDetail.createdDate = currentdatetime;
                db.PaymentDetails.Add(paymentDetail);
                db.SaveChanges();
                db.BuyPolicies.Add(buyPolicy);
                db.SaveChanges();
                Session["Success"]="Policy bought successfully";
                return RedirectToAction("getPolicy", "Customer");
            }
            catch(Exception e)
            {
                Session["Success"] = "Problem in payment try again."+e;
                return RedirectToAction("getPolicy", "Customer");
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
            catch(Exception e)
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
                var customer = db.USERS.Where(x => x.Email == user.Email && x.Id!=userid).FirstOrDefault();
                if (customer == null)
                {
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    Session["Success"]="Customer Details Updated Successfully";
                    return View();
                }
                Session["Success"] = "Email Id already exists";
                return View();

            }
            catch(Exception e)
            {
                ViewBag.Message = "Problem in updating the Customer details: "+e;
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
                return View(complaintList);
            }
            catch(Exception e)
            {
                Session["Success"] = "Problem in getting complaints" + e;
                return RedirectToAction("getPolicy", "Customer");
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

        public ActionResult myPolicies()
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                List<Policy> policies = db.Policies.ToList();
                List<BuyPolicy> buyPolicies = db.BuyPolicies.ToList();
                int id = (int)Session["Id"];
                var policyList = from p in policies
                                 join b in buyPolicies on p.Id equals b.PolicyId
                                 where b.UserId == id
                                 select new getPolicyViewModel
                                 {
                                     buyPolicy = b,
                                     policy = p
                                 };
                return View(policyList);
            }
            catch(Exception e)
            {
                Session["Success"] = "Problem in getting policies" + e;
                return RedirectToAction("getPolicy", "Customer");
            }
            
        }

        public ActionResult payPolicy(int id)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                PoliciesTransaction policiesTransaction = new PoliciesTransaction();
                policiesTransaction.UserId = (int)Session["Id"];
                policiesTransaction.PolicyId = id;
                var polices = db.Policies.Where(x => x.Id == id).FirstOrDefault();
                int userid = (int)Session["Id"];
                var buyPolcies = db.BuyPolicies.Where(x => x.PolicyId == id && x.UserId == userid).FirstOrDefault();
                policiesTransaction.Amount = polices.PremiumAmount;
                policiesTransaction.ActualPremiumDate = buyPolcies.CreatedDate;
                Session["trasaction"] = policiesTransaction;/*
            db.PoliciesTransactions.Add(policiesTransaction);
            db.SaveChanges();
            ViewBag.Message = "Transaction completed successfully";*/
                return View();
            }
            catch(Exception e)
            {
                Session["Success"] = "Problem in payment" + e;
                return RedirectToAction("myPolicies", "Customer");
            }
            
        }
        [HttpPost]
        public ActionResult payPolicy(PaymentDetail paymentDetail)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                PoliciesTransaction policiesTransaction = new PoliciesTransaction();
                policiesTransaction = (PoliciesTransaction)Session["trasaction"];
                DateTime currentDatetime = DateTime.Now;
                policiesTransaction.ActualPaymentDate = currentDatetime;
                db.PoliciesTransactions.Add(policiesTransaction);
                db.SaveChanges();
                Session["Success"] = "Transaction completed successfully";
                return RedirectToAction("myPolicies", "Customer");
            }
            catch(Exception e)
            {
                Session["Success"] = "Problem in payment" + e;
                return RedirectToAction("myPolicies", "Customer");
            }
            
        }

        public ActionResult claimPolicy(int id)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                int userid = (int)Session["Id"];
                var claimdetails = db.PoliciesClaims.Where(x => x.PolicyId == id && x.UserId == userid).FirstOrDefault();
                if (claimdetails == null)
                {
                    PoliciesClaim claim = new PoliciesClaim();
                    claim.PolicyId = id;
                    claim.ClaimStatusID = 1;

                    claim.UserId = userid;
                    Session["claim"] = claim;
                    return View();
                }
                Session["Success"] = "Policy already claimed";
                return RedirectToAction("myPolicies", "Customer");

            }
            catch(Exception e)
            {
                Session["Success"] = "Problem in claiming policy" + e;
                return RedirectToAction("myPolicies", "Customer");
            }
            
        }

        [HttpPost]
        public ActionResult claimPolicy(PoliciesClaim policiesClaim)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                PoliciesClaim claim = new PoliciesClaim();
                claim = (PoliciesClaim)Session["claim"];
                claim.ClaimAmount = policiesClaim.ClaimAmount;
                claim.Reason = policiesClaim.Reason;
                DateTime currentDatetime = DateTime.Now;
                claim.CreatedDate = currentDatetime;
                db.PoliciesClaims.Add(claim);
                db.SaveChanges();
                Session["Success"] ="Policy clamied successfully";
                return RedirectToAction("myPolicies","Customer");
            }
            catch(Exception e)
            {
                Session["Success"] = "Policy not claimed"+e;
                return RedirectToAction("myPolicies", "Customer");
            }
            
        }

        public ActionResult renewPolicy(int id)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                var buyPolicy = db.BuyPolicies.Where(x => x.Id == id).FirstOrDefault();
                DateTime currentDatetime = DateTime.Now;
                buyPolicy.CreatedDate = currentDatetime;
                db.SaveChanges();
                Session["Success"] = "Policy Renewed successfully";
                return RedirectToAction("myPolicies", "Customer");
            }
            catch(Exception e)
            {
                Session["Success"] = "Problem in renewing policy" + e;
                return RedirectToAction("myPolicies", "Customer");
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
                var transactionHistory = db.PoliciesTransactions.Where(x => x.UserId == id).ToList();
                return View(transactionHistory);
            }
            catch(Exception e)
            {
                Session["Success"] = "Problem in getting transactions of policy" + e;
                return RedirectToAction("myPolicies", "Customer");
            }
            
        }



    }
}