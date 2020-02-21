using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MYBUSINESS.Models;
namespace MYBUSINESS.Controllers
{
    public class UserRegisterController : Controller
    {
        private BusinessContext db = new BusinessContext();
        // GET: UserRegister

        public ActionResult Register()
        {
            int maxId = db.Employees.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewCustId = maxId;
            return View();

        }

        [HttpPost]
       [ValidateAntiForgeryToken]
        public ActionResult Register(Business rgstr)
        
        {

            
            if (ModelState.IsValid)
            {
                
                db.Businesses.Add(rgstr);
                db.SaveChanges();
             
                return RedirectToAction("Login","UserManagement");
            }

            //return View(customer);
            return View(rgstr);
        }
    }
}