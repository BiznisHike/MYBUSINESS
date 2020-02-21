using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MYBUSINESS.Models;

namespace MYBUSINESS.Controllers
{
   
    public class BusinessController : Controller
    {
        private BusinessContext db = new BusinessContext();

        public ActionResult Edit(decimal id)
        {
            Business CurrentUser = (Business)Session["CurrentUser"];
            id = CurrentUser.Id;
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            Business business = db.Businesses.Find(id);
            if (business == null)
            {
                return HttpNotFound();
            }
            return View(business);
        }
             [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "email,Password")] Business userChanges, FormCollection fc)
        {
            string oldPass = fc["OldPassword"];
            string pass1 = fc["Password1"];
            string pass2 = fc["Password2"];

            Business CurrentUser = (Business)Session["CurrentUser"];

            if (CurrentUser.email == userChanges.email && CurrentUser.Password == Encryption.Encrypt(oldPass) && pass1 == pass2)
            {
                userChanges.Id = CurrentUser.Id;
                //userChanges.Login = userChanges.Login;
                //userChanges.Password = Encryption.Encrypt(pass2);

                if (ModelState.IsValid)
                {

                    //db.Entry(userChanges).State = EntityState.Modified;
                    db.Businesses.Add(userChanges);
                    db.SaveChanges();
                    Session.Add("CurrentUser", userChanges);

                    return RedirectToAction("Create", "SOSR");
                }
            }

            ViewBag.Error = "Password does not match";
            return View(userChanges);
        }
    }
    }
