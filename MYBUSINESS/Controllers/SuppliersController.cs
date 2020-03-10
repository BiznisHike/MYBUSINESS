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
    public class SuppliersController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private IQueryable<Supplier> _dbFilteredSuppliers;
        protected override void Initialize(RequestContext requestContext)
        {

            base.Initialize(requestContext);

            Business CurrentBusiness = (Business)Session["CurrentBusiness"];
          
            _dbFilteredSuppliers = db.Suppliers.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
        }
        // GET: Suppliers
        public ActionResult Index(string id)
        {
           
            return View(_dbFilteredSuppliers.ToList());
        }

        // GET: Suppliers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // GET: Suppliers/Create
        public ActionResult Create()
        {
            decimal maxId = db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewSuppId = maxId;
            return View();
        }

        // POST: Suppliers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "bizId,Id,Name,Address,Balance")] Supplier supplier,String AddAnother)
        {
            Business CurrentBusiness = (Business)Session["CurrentBusiness"];
            supplier.bizId = CurrentBusiness.Id;
            if (ModelState.IsValid)
            {
                if (supplier.Balance == null)
                {
                    supplier.Balance = 0;
                }
                db.Suppliers.Add(supplier);
                db.SaveChanges();
                if (!string.IsNullOrEmpty(AddAnother))

                {
                    return RedirectToAction("Create");
                }
                else { return RedirectToAction("Index"); }
            }

            return View(supplier);
        }

        // GET: Suppliers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Address,Balance")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supplier).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(supplier);
        }

        //// GET: Suppliers/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Supplier supplier = db.Suppliers.Find(id);
        //    if (supplier == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(supplier);
        //}

        //// POST: Suppliers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Supplier supplier = db.Suppliers.Find(id);
        //    db.Suppliers.Remove(supplier);
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
