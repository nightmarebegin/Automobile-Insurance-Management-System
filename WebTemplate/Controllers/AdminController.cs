using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebTemplate.Models;

namespace WebTemplate.Controllers
{
    public class AdminController : Controller
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
        // GET: Admin
        public ActionResult Index()
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                var customerList = db.USERS.Where(x => x.RoleId == 1).ToList();
                if (customerList == null)
                {
                    Session["Success"] = "No customers available yet";
                }
                return View(customerList);
            }
            catch(Exception e)
            {
                Session["Success"] = "Problem in getting customers list"+e;
                return RedirectToAction("ManageProfile", "Admin");
            }
            
                
            
            
            
        }
        public ActionResult agentList()
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                var agentList = db.USERS.Where(x => x.RoleId == 2).ToList();
                if (agentList == null)
                {
                    Session["Success"] = "No agents available yet";
                }
                return View(agentList);
            }
            catch (Exception e)
            {
                Session["Success"] = "Problem in getting agent list" + e;
                return RedirectToAction("ManageProfile", "Admin");
            }

        }
        public ActionResult viewDetailsCustomer(int userid)
        {
            if (SessionCheck() != true)
            {
                return RedirectToAction("Index", "Login");
            }
            var customerDetails = db.USERS.Where(x => x.Id == userid).FirstOrDefault();
            return View(customerDetails);
        }
        public ActionResult viewDetailsAgent(int userid)
        {
            if (SessionCheck() != true)
            {
                return RedirectToAction("Index", "Login");
            }
            var agentDetails = db.USERS.Where(x => x.Id == userid).FirstOrDefault();
            return View(agentDetails);
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

        public ActionResult editCustomer(int userid)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                int id = (int)Session["Id"];
                var customerData = db.USERS.Where(x => x.Id == userid).FirstOrDefault();
                return View(customerData);
            }
            catch(Exception e)
            {
                return RedirectToAction("Index", "Admin");
            }
            
        }

        [HttpPost]
        public ActionResult editCustomer(USER user)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                
                var customer = db.USERS.Where(x => x.Email == user.Email && x.Id != user.Id).FirstOrDefault();
                if (customer == null)
                {
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    Session["Success"] = "Customer Details Updated Successfully";
                    return RedirectToAction("Index", "Admin");
                }
                Session["Success"] = "Email Id already Exists";
                return View();
            }
            catch (Exception e)
            {
                Session["Success"] = "Problem in updating the customer details: " + e;
                return RedirectToAction("Index", "Admin");
            }

        }
        public ActionResult editAgent(int userid)
        {
            if (SessionCheck() != true)
            {
                return RedirectToAction("Index", "Login");
            }
            int id = (int)Session["Id"];
            var agentData = db.USERS.Where(x => x.Id == userid).FirstOrDefault();
            return View(agentData);
        }

        [HttpPost]
        public ActionResult editAgent(USER user)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                
                var customer = db.USERS.Where(x => x.Email == user.Email && x.Id != user.Id).FirstOrDefault();
                if (customer == null)
                {
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    Session["Success"] = "Agent Details Updated Successfully";
                    return RedirectToAction("agentList", "Admin");
                }
                Session["Success"] = "Email Id already Exists";
                return View();

            }
            catch (Exception e)
            {
                Session["Success"] = "Problem in updating the Agent details: " + e;
                return RedirectToAction("agentList", "Admin");
            }

        }
        public ActionResult editPolicy(int policyid)
        {
            if (SessionCheck() != true)
            {
                return RedirectToAction("Index", "Login");
            }
            int id = (int)Session["Id"];
            var policyData = db.Policies.Where(x => x.Id == policyid).FirstOrDefault();
            return View(policyData);
        }

        [HttpPost]
        public ActionResult editPolicy(Policy policy)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                db.Entry(policy).State = EntityState.Modified;
                db.SaveChanges();
                Session["Success"] = "Policy Details Updated Successfully";
                return RedirectToAction("getPolicy","Admin");
            }
            catch (Exception e)
            {
                Session["Success"] = "Problem in updating the policy details: " + e;
                return RedirectToAction("getPolicy", "Admin");
            }

        }

        public ActionResult ManageComplaints()
        {
            if (SessionCheck() != true)
            {
                return RedirectToAction("Index", "Login");
            }
            var complaintList = db.Complaints.ToList();
            if (complaintList == null)
            {
                Session["Success"] = "Complaint list is empty";
            }
            return View(complaintList);
        }
        public ActionResult ManageFeedbacks()
        {
            if (SessionCheck() != true)
            {
                return RedirectToAction("Index", "Login");
            }
            var complaintList = db.Feedbacks.ToList();
            if (complaintList == null)
            {
                Session["Success"] = "Feedback list is empty";
            }
            return View(complaintList);
        }
        public ActionResult transaction()
        {
            if (SessionCheck() != true)
            {
                return RedirectToAction("Index", "Login");
            }
            
            var transactionHistory = db.PoliciesTransactions.ToList();
            if (transactionHistory == null)
            {
                Session["Success"] = "Transaction list is empty";
            }
            return View(transactionHistory);
        }

        public ActionResult getPolicy()
        {
            if (SessionCheck() != true)
            {
                return RedirectToAction("Index", "Login");
            }
            var data = db.Policies.ToList();
            if (data == null)
            {
                Session["Success"] = "Policy list is empty";
            }
            return View(data);

        }

        public ActionResult getPolicyDetails(int policyid)
        {
            if (SessionCheck() != true)
            {
                return RedirectToAction("Index", "Login");
            }
            var policyData = db.Policies.Where(x => x.Id == policyid).FirstOrDefault();
            return View(policyData);
        }

        public ActionResult deletePolicy(int policyid)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                var policyById = db.Policies.Where(x => x.Id == policyid).FirstOrDefault();
;                db.Entry(policyById).State = EntityState.Deleted;
                 db.SaveChanges();
                Session["Success"] = "Policy Deleted Successfully";
                return RedirectToAction("getPolicy", "Admin");
            }
            catch(Exception e)
            {
                Session["Success"] = "Problem in deleting policy"+e;
                return RedirectToAction("getPolicy", "Admin");
            }
            
        }
        public ActionResult deleteAgent(int userid)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                var userById = db.USERS.Where(x => x.Id == userid).FirstOrDefault();
                ; db.Entry(userById).State = EntityState.Deleted;
                db.SaveChanges();
                Session["Success"] = "Agent Deleted Successfully";
                return RedirectToAction("agentList", "Admin");
            }
            catch (Exception e)
            {
                Session["Success"] = "Problem in deleting agent" + e;
                return RedirectToAction("agentList", "Admin");
            }

        }
        public ActionResult deleteCustomer(int userid)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                var userById = db.USERS.Where(x => x.Id == userid).FirstOrDefault();
                db.Entry(userById).State = EntityState.Deleted;
                db.SaveChanges();
                Session["Success"] = "Customer Deleted Successfully";
                return RedirectToAction("Index", "Admin");
            }
            catch (Exception e)
            {
                Session["Success"] = "Problem in deleting customer" + e;
                return RedirectToAction("Index", "Admin");
            }

        }

        public ActionResult claimRequest()
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                var claimRequest = db.PoliciesClaims.ToList();
                if (claimRequest == null)
                {
                    Session["Success"] = "Policy claim list is empty";
                }
                return View(claimRequest);
            }
            catch (Exception e)
            {
                Session["Success"] = "Problem in claim request" + e;
                return View();
            }
        }

        public ActionResult claimRequestDetails(int claimid)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                var claimRequest = db.PoliciesClaims.Where(x=>x.Id==claimid).FirstOrDefault();
                return View(claimRequest);
            }
            catch (Exception e)
            {
                Session["Success"] = "Problem in getting details" + e;
                return View();
            }
        }
        [HttpPost]
        public ActionResult claimRequestDetails(PoliciesClaim policiesClaim)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                db.Entry(policiesClaim).State = EntityState.Modified;
                db.SaveChanges();
                Session["Success"] = "Claim request Updated Successfully";
                return RedirectToAction("claimRequest", "Admin");
            }
            catch (Exception e)
            {
                Session["Success"] = "Problem in updating details" + e;
                return View();
            }
        }

        public ActionResult createPolicy()
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                
                
                return View();
            }
            catch (Exception e)
            {
                Session["Success"] = "Problem " + e;
                return View();
            }
        }
        [HttpPost]
        public ActionResult createPolicy(Policy policy)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                DateTime current = DateTime.Now;
                policy.CreatedDate = current;
                db.Policies.Add(policy);
                db.SaveChanges();
                Session["Success"] = "Policy created Successfully";
                return RedirectToAction("getPolicy", "Admin");
            }
            catch (Exception e)
            {
                Session["Success"] = "policy not created";
                return RedirectToAction("getPolicy", "Admin");
            }
        }
        public ActionResult createAgent()
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }


                return View();
            }
            catch (Exception e)
            {
                Session["Success"] = "Problem " + e;
                return View();
            }
        }
        [HttpPost]
        public ActionResult createAgent(USER agent)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                int userid = (int)Session["Id"];
                var agents = db.USERS.Where(x => x.Email == agent.Email && x.Id != userid).FirstOrDefault();
                if (agents == null)
                {
                    DateTime current = DateTime.Now;
                    agent.CreatedDate = current;
                    agent.RoleId = 2;
                    db.USERS.Add(agent);
                    db.SaveChanges();
                    Session["Success"] = "Agent created Successfully";
                    return RedirectToAction("agentList", "Admin");
                }
                Session["Success"] = "Email Id already exists";
                return View();
                
            }
            catch (Exception e)
            {
                Session["Success"] = "Agent not created";
                return RedirectToAction("agentList", "Admin");
            }
        }

        public ActionResult CustomerAgents()
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                var agentList = db.USERS.Where(x=>x.RoleId==2).ToList();
                SelectList list = new SelectList(agentList, "Id", "Name");
                ViewBag.agentlistdetails = list;
                var customerList = db.USERS.Where(x => x.RoleId == 1).ToList();
                SelectList list1 = new SelectList(customerList, "Id", "Name");
                ViewBag.customerlistdetails = list1;
                return View();
            }
            catch (Exception e)
            {
                Session["agent_error"] = "Problem " + e;
                return View();
            }
        }


        [HttpPost]
        public ActionResult CustomerAgents(CustomerAgent customeragent)
        {
            try
            {
                if (SessionCheck() != true)
                {
                    return RedirectToAction("Index", "Login");
                }
                var list = db.CustomerAgents.Where(c => c.UserId == customeragent.UserId).FirstOrDefault();
                if (list == null)
                {
                    DateTime current = DateTime.Now;
                    customeragent.CreatedDate = current;

                    db.CustomerAgents.Add(customeragent);
                    db.SaveChanges();
                    Session["agent_success"] = "Agent assigned to customer Successfully";
                    return RedirectToAction("CustomerAgents", "Admin");
                }
                Session["agent_success"] = "Agent already assigned for this customer";
                return RedirectToAction("CustomerAgents", "Admin");

            }
            catch (Exception e)
            {
                Session["agent_error"] = "Agent assigned to customer not created";
                return RedirectToAction("CustomerAgents", "Admin");
            }
        }
    }
}