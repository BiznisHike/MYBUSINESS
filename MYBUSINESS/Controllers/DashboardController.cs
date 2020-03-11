using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Microsoft.Reporting.WebForms;
using MYBUSINESS.CustomClasses;
using System.Web.Routing;
using MYBUSINESS.Models;

namespace MYBUSINESS.Controllers
{
    public class DashboardController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private IQueryable<Product> _dbFilteredProducts;
        private IQueryable<Supplier> _dbFilteredSuppliers;
        private IQueryable<Account> _dbFilteredAccounts;
        private IQueryable<PO> _dbFilteredPO;
        private IQueryable<SO> _dbFilteredSO;
        private IQueryable<Employee> _dbFilteredEmployees;
        private IQueryable<Department> _dbFilteredDepartments;
        private IQueryable<Customer> _dbFilteredCustomers;

        protected override void Initialize(RequestContext requestContext)
        {
            //Employee employee1 = TempData["mydata"] as Employee;
            //Employee employee = ViewBag.data;
            base.Initialize(requestContext);
            //decimal BusinessId = decimal.Parse(this.ControllerContext.RouteData.Values["CurrentBusiness"].ToString());
            Business CurrentBusiness = (Business)Session["CurrentBusiness"];
            _dbFilteredSuppliers = db.Suppliers.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
            _dbFilteredProducts = db.Products.AsQueryable().Where(x => x.Supplier.bizId == CurrentBusiness.Id);
            _dbFilteredPO = db.POes.AsQueryable().Where(x => x.Supplier.bizId == CurrentBusiness.Id);
            _dbFilteredAccounts = db.Accounts.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
            _dbFilteredSO = db.SOes.AsQueryable().Where(x => x.Employee.Department.bizId == CurrentBusiness.Id);
            _dbFilteredEmployees = db.Employees.AsQueryable().Where(x => x.Department.bizId == CurrentBusiness.Id);
            _dbFilteredDepartments = db.Departments.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
            _dbFilteredCustomers = db.Customers.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
            _dbFilteredProducts = db.Products.AsQueryable().Where(x => x.Supplier.bizId == CurrentBusiness.Id);
        }
        // GET: Dashboard
        public ActionResult Index()
        {
            decimal SO = _dbFilteredSO.Count();
            ViewBag.SO = SO;
            decimal PO = _dbFilteredPO.Count();
            ViewBag.PO = PO;
            decimal Products = _dbFilteredProducts.Count();
            ViewBag.Products = Products;
            decimal suppliers = _dbFilteredSuppliers.Count();
            ViewBag.suppliers = suppliers;
            decimal customers = _dbFilteredCustomers.Count();
            ViewBag.customers = customers;
            decimal employees = _dbFilteredEmployees.Count();
            ViewBag.employees = employees;
            decimal departments = _dbFilteredDepartments.Count();
            ViewBag.departments = departments;
            decimal accounts = _dbFilteredAccounts.Count();
            ViewBag.accounts = accounts;
           

            return View();
        }
    }
}