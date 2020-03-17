using MYBUSINESS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MYBUSINESS.Controllers
{
    public class BusinessRegisterController : Controller
    {
        private BusinessContext db = new BusinessContext();
        // GET: BusinessRegister
        public ActionResult Register()

        {


            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(BusinessRegisterVM registerVM)

        {
            Employee User = db.Employees.SingleOrDefault(u => u.Email == registerVM.employee.Email);
            Business BName = db.Businesses.SingleOrDefault(usr => usr.Name == registerVM.business.Name);
            //var user = db.Employees.SingleOrDefault(usr => ((usr.Email == registerVM.employee.Email)));
            
            if (BName!=null && User!=null)
            {
                ViewBag.NameError = "User Name Already Exist!";
                ViewBag.EmailError = "Email Address Already Exist!";
                return View("Register");
            }
            else if (BName != null && User == null) {
                ViewBag.NameError = "User Name Already Exist!";
                return View("Register");

            }else if (BName == null && User != null) {
                ViewBag.EmailError = "Email Address Already Exist!";
                return View("Register");
            }
            else
            {


                decimal maxId = db.Employees.DefaultIfEmpty().Max(e => e == null ? 0 : e.Id);
                maxId += 1;

                decimal maxBizId = db.Businesses.DefaultIfEmpty().Max(e => e == null ? 0 : e.Id);
                maxBizId += 1;

                Business business = new Business();
                business.Id = maxBizId;
                business.Email = registerVM.employee.Email;
                business.Name = registerVM.business.Name;
                business.phone = registerVM.business.phone;
                db.Businesses.Add(business);
                db.SaveChanges();

                Department department = new Department();

                department.bizId = maxBizId;
                department.CreateDate = DateTime.Now;
                decimal maxDeptId = db.Departments.DefaultIfEmpty().Max(e => e == null ? 0 : e.Id);
                maxDeptId += 1;
                department.Id = maxDeptId;
                department.Name = "Admin";
                db.Departments.Add(department);
                db.SaveChanges();


                Employee employee = new Employee();
                int pos = registerVM.employee.Email.IndexOf("@");

                employee.Login = registerVM.employee.Email.Substring(0, pos);
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    if (file.ContentLength > 0)
                    {
                        String FileName = Path.GetFileNameWithoutExtension(file.FileName);

                        FileName = registerVM.business.Name + maxBizId;
                        string Extention = Path.GetExtension(file.FileName);
                        FileName = FileName + Extention;
                        registerVM.employee.ImgPath = "~/Image/" + FileName;
                        FileName = Path.Combine(Server.MapPath("~/Image/"), FileName);
                        file.SaveAs(FileName);
                        employee.ImgPath = registerVM.employee.ImgPath;

                    }
                }
                employee.Id = maxId;
                employee.bizId = maxBizId.ToString();
                employee.FirstName = registerVM.employee.FirstName;
                employee.LastName = registerVM.employee.LastName;
                employee.Email = registerVM.employee.Email;
                employee.Password = Encryption.Encrypt(registerVM.employee.Password);
                employee.DepartmentId = maxDeptId;
                employee.RegistrationDate = DateTime.Now;
                db.Employees.Add(employee);
                db.SaveChanges();
            };

            
                return RedirectToAction("Login", "UserManagement");
            }
        }
    }
