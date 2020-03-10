using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using MYBUSINESS.Models;

namespace MYBUSINESS.Controllers
{
    public class CustomersController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private IQueryable<Customer> _dbFilteredCustomers;
        protected override void Initialize(RequestContext requestContext)
        {

            base.Initialize(requestContext);

            Business CurrentBusiness = (Business)Session["CurrentBusiness"];
            _dbFilteredCustomers = db.Customers.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
        }
        // GET: Customers
        public ActionResult Index(string id)
        {

            return View(_dbFilteredCustomers.ToList());
        }

        // GET: Customers/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            //int maxId = db.Customers.Max(p => p.Id);
            decimal BCId = _dbFilteredCustomers.Count();
            BCId += 1;
            ViewBag.SuggestedNewCustId = BCId;
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "bizId,vId,Name,Address,Balance")] Customer customer, String AddAnother)
        {
            decimal maxId = db.Customers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            Business CurrentBusiness = (Business)Session["CurrentBusiness"];
            customer.bizId = CurrentBusiness.Id;
            customer.Id = maxId;
            if (ModelState.IsValid)
            {
                if (customer.Balance == null)
                {
                    customer.Balance = 0;

                }
                db.Customers.Add(customer);
                db.SaveChanges();
                if (!string.IsNullOrEmpty(AddAnother))

                {
                    return RedirectToAction("Create");
                }
                else { return RedirectToAction("Index"); }
            }

            //if ((TempData["Controller"]).ToString() == "SOSR" && (TempData["Action"]).ToString() == "Create")
            //{
            //    return RedirectToAction("Create", "SOSR");

            //}
            //else
            //{
            //    return View(customer);
            //}

            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Address,Balance")] Customer customer)
        {
            if (ModelState.IsValid)
            {

                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        //// GET: Customers/Delete/5
        //public ActionResult Delete(decimal id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Customer customer = db.Customers.Find(id);
        //    if (customer == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(customer);
        //}

        //// POST: Customers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(decimal id)
        //{
        //    Customer customer = db.Customers.Find(id);
        //    db.Customers.Remove(customer);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
