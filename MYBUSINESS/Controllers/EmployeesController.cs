using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.IO;

using MYBUSINESS.Models;

namespace MYBUSINESS.Controllers
{
    public class EmployeesController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private IQueryable<Employee> _dbFilteredEmployees;
        private IQueryable<Department> _dbFilteredDepartments;
        public EmployeesController()
        {
            //Business CurrentBusiness = (Business)Session["CurrentBusiness"];
            // db.Employees = (DbSet<Employee>)db.Employees.AsQueryable().Where(x => x.Department.bizId == CurrentBusiness.Id);
            // db.Departments = (DbSet<Department>)db.Departments.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
        }
        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);

            Business CurrentBusiness = (Business)Session["CurrentBusiness"];

            _dbFilteredEmployees = db.Employees.AsQueryable().Where(x => x.Department.bizId == CurrentBusiness.Id);
            _dbFilteredDepartments = db.Departments.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
        }
        //GET: Employees
        public ActionResult Index()
        {
            return View(_dbFilteredEmployees.ToList());
        }

        //// GET: Employees/Details/5
        //public ActionResult Details(decimal id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Employee employee = db.Employees.Find(id);
        //    if (employee == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(employee);
        //}

        // GET: Employees/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Employees/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "Id,FirstName,LastName,Gender,Login,Password,Email,EmployeeTypeId,RightId,RankId,DepartmentId,Designation,Probation,RegistrationDate,Casual,Earned,IsActive,tblCreatedDate,tblModifiedDate")] Employee employee)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Employees.Add(employee);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(employee);
        //}

        // GET: Employees/Edit/5
        public ActionResult Create()
        {
            ViewBag.Department = _dbFilteredDepartments;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee newEmployee, String AddAnother)
        {
            Employee user = db.Employees.SingleOrDefault(x => x.Email == newEmployee.Email);
            if (user != null)
            {
                ViewBag.Error = "Employee Id Already Exist!";
                ViewBag.Department = _dbFilteredDepartments;
                return View("Create");

            }
            decimal maxId = db.Employees.DefaultIfEmpty().Max(e => e == null ? 0 : e.Id);
            maxId += 1;

            Employee Nemployee = new Employee();
            if (Request.Files.Count > 0)
            {
                HttpPostedFileBase file = Request.Files[0];
                if (file.ContentLength > 0)
                {
                    String FileName = Path.GetFileNameWithoutExtension(file.FileName);

                    FileName = newEmployee.FirstName + maxId;
                    string Extention = Path.GetExtension(file.FileName);
                    FileName = FileName + Extention;
                    newEmployee.ImgPath = "~/Image/" + FileName;
                    FileName = Path.Combine(Server.MapPath("~/Image/"), FileName);
                    file.SaveAs(FileName);
                    Nemployee.ImgPath = newEmployee.ImgPath;

                }
            }
            //if (!string.IsNullOrEmpty(newEmployee.ImgPath))
            //{
            //    String FileName = Path.GetFileNameWithoutExtension(newEmployee.ImageFile.FileName);
            //    string Extention = Path.GetExtension(newEmployee.ImageFile.FileName);
            //    FileName = FileName + DateTime.Now.ToString("yymmssfff") + Extention;
            //    newEmployee.ImgPath = "~/Image/" + FileName;
            //    FileName = Path.Combine(Server.MapPath("~/Image/"), FileName);
            //    newEmployee.ImageFile.SaveAs(FileName);
            //    Nemployee.ImgPath = newEmployee.ImgPath;
            //}
            newEmployee.Id = maxId;
            int pos = newEmployee.Email.IndexOf("@");
            newEmployee.Login = newEmployee.Email.Substring(0, pos);


            newEmployee.RegistrationDate = DateTime.Now;
            newEmployee.Password = Encryption.Encrypt(newEmployee.Password);

            db.Employees.Add(newEmployee);
            db.SaveChanges();
            if (!string.IsNullOrEmpty(AddAnother))

            {
                return RedirectToAction("Create");
            }
            else { return RedirectToAction("Index"); }

        }
        public ActionResult Edit(decimal id)
        {
            Employee CurrentUser = (Employee)Session["CurrentUser"];
            id = CurrentUser.Id;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = db.Employees.Find(id);
           
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //public ActionResult Edit([Bind(Include = "Login,Password")] Employee employee)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Email,Password,bizId,DepartmentId,ImgPath")] Employee userChanges, FormCollection fc)
        {
            string oldPass = fc["OldPassword"];
            string pass1 = fc["Password1"];
            string pass2 = fc["Password2"];

            Employee CurrentUser = (Employee)Session["CurrentUser"];

            if (CurrentUser.Email == userChanges.Email && CurrentUser.Password == Encryption.Encrypt(oldPass) && pass1 == pass2)
            {
                userChanges.Id = CurrentUser.Id;
                //userChanges.Login = userChanges.Login;
                userChanges.Password = Encryption.Encrypt(pass2);
                userChanges.FirstName = CurrentUser.FirstName;
                userChanges.LastName = CurrentUser.LastName;
                userChanges.RegistrationDate = CurrentUser.RegistrationDate;
                userChanges.UpdateDate = DateTime.Now;
                userChanges.Login = CurrentUser.Email.Split('@').FirstOrDefault();

                if (ModelState.IsValid)
                {

                    db.Entry(userChanges).State = EntityState.Modified;
                    db.SaveChanges();
                    Session.Add("CurrentUser", userChanges);
                    //ViewBag.IsReturn = "false";
                    //return RedirectToAction("Create","SOSR", "false");
                    return RedirectToAction("Create", "SOSR", new { IsReturn = "false" });//change it from 'if condtion' to here
                }
            }

            ViewBag.Error = "Password does not match";
            return View(userChanges);
        }

        // GET: Employees/Delete/5
        //public ActionResult Delete(decimal id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Employee employee = db.Employees.Find(id);
        //    if (employee == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(employee);
        //}

        //// POST: Employees/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(decimal id)
        //{
        //    Employee employee = db.Employees.Find(id);
        //    db.Employees.Remove(employee);
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
