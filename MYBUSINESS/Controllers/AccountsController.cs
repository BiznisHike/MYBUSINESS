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
    public class AccountsController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private IQueryable<Account> _dbFilteredAccounts;

        public AccountsController()
        {
            //Business CurrentBusiness = (Business)Session["CurrentBusiness"];
            //db.Accounts = (DbSet<Account>)db.Accounts.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
            //db.Businesses = (DbSet<Business>)db.Businesses.AsQueryable().Where(x => x.Id == CurrentBusiness.Id);
        }
        protected override void Initialize(RequestContext requestContext)
        {

            base.Initialize(requestContext);

            Business CurrentBusiness = (Business)Session["CurrentBusiness"];
            _dbFilteredAccounts = db.Accounts.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
        }
        // GET: Accounts
        public ActionResult Index()
        {
            var accounts = _dbFilteredAccounts;
            return View(accounts.ToList());
        }

        // GET: Accounts/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {

            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Account account, String AddAnother)
        {
            Business CurrentBusiness = (Business)Session["CurrentBusiness"];
            if (ModelState.IsValid)
            {
                decimal maxAccId = db.Accounts.DefaultIfEmpty().Max(a => a == null ? 0 : a.Id);
                maxAccId += 1;

               
               account.bizId = CurrentBusiness.Id;
               account.Id = maxAccId;
     
                account.CreateDate = DateTime.Now;
                db.Accounts.Add(account);
                db.SaveChanges();
                if (!string.IsNullOrEmpty(AddAnother))

                { return RedirectToAction("Create");
                }
                else{ return RedirectToAction("Index"); }

            }

            ViewBag.bizId = CurrentBusiness.Name;
            return View(account);
        }

        // GET: Accounts/Edit/5
        public ActionResult Edit(decimal id)
        {
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.time = DateTime.Now;
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Remarks,CreateDate,UpdateDate,bizId")] Account account)
        {
            
            if (ModelState.IsValid)
            {
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(account);
        }

        // GET: Accounts/Delete/5
        public ActionResult Delete(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(decimal id)
        {
            Account account = db.Accounts.Find(id);
            db.Accounts.Remove(account);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

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
