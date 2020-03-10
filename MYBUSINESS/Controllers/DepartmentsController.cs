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
    public class DepartmentsController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private IQueryable<Department> _dbFilteredDepartments;

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
          
            Business CurrentBusiness = (Business)Session["CurrentBusiness"];

            _dbFilteredDepartments = db.Departments.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
        }
        // GET: Departments
        public ActionResult Index()
        {
            return View(_dbFilteredDepartments.ToList());
        }

        // GET: Departments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Department department,String AddAnother)
        {
            Business CurrentBusiness = (Business)Session["CurrentBusiness"];
            if (ModelState.IsValid)
            {
                decimal maxDeptId = db.Departments.DefaultIfEmpty().Max(e => e == null ? 0 : e.Id);
                maxDeptId += 1;
                Department Ndepartment = new Department();
                Ndepartment.bizId = CurrentBusiness.Id;
                Ndepartment.Name = department.Name;
                Ndepartment.Remarks=department.Remarks;
                Ndepartment.Id = maxDeptId;
                Ndepartment.CreateDate = DateTime.Now;
                db.Departments.Add(Ndepartment);
                db.SaveChanges();
                if (!string.IsNullOrEmpty(AddAnother))

                {
                    return RedirectToAction("Create");
                }
                else { return RedirectToAction("Index"); }
            }

            return View(department);
        }

        // GET: Departments/Edit/5
        public ActionResult Edit(int? id)
        {
          
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
           
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "bizId,Id,Name,Remarks,CreateDate,UpdateDate")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Entry(department).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(department);
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Department department = db.Departments.Find(id);
            db.Departments.Remove(department);
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
