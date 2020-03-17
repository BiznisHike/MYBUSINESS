using MYBUSINESS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MYBUSINESS.Controllers
{
    public class ExpensesController : Controller
    {
        private BusinessContext db = new BusinessContext();
        // GET: Expenses
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
            var dtStartDate = new DateTime(PKDate.Year, PKDate.Month,PKDate.Day);
          
            ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(expense expense ,[Bind(Prefix = "ExpenseDetail", Include = "ExpenseName,Amount")] List<ExpenseDetail> expenseDetails, FormCollection collection)

        {
            decimal maxExpncId = db.expenses.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxExpncId += 1;
            //decimal maxExpncDId = db.ExpenseDetails.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            //maxExpncDId += 1;




            decimal TotalAmount = 0;

            foreach (ExpenseDetail ed in expenseDetails)
            {
                TotalAmount += ed.Amount.Value;
                ed.ExpenseId = maxExpncId;
                db.ExpenseDetails.Add(ed);
            }
            expense.Id = maxExpncId;
            Business CurrentBusiness = (Business)Session["CurrentBusiness"];
            expense.BizId = CurrentBusiness.Id;
            expense.expenseTotal = TotalAmount;
            db.expenses.Add(expense);
            db.SaveChanges();

            return RedirectToAction("Create");
        }
    }
}