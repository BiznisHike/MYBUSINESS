﻿using System;
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
    public class POPRController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private IQueryable<Product> _dbFilteredProducts;
        private IQueryable<Supplier> _dbFilteredSuppliers;
        private IQueryable<Account> _dbFilteredAccounts;
        private IQueryable<PO> _dbFilteredPO;
        private IQueryable<POD> _dbFilteredPoD;
        protected override void Initialize(RequestContext requestContext)
        {
            //Employee employee1 = TempData["mydata"] as Employee;
            //Employee employee = ViewBag.data;
            base.Initialize(requestContext);
            //decimal BusinessId = decimal.Parse(this.ControllerContext.RouteData.Values["CurrentBusiness"].ToString());
            Business CurrentBusiness = (Business)Session["CurrentBusiness"];
            _dbFilteredSuppliers = db.Suppliers.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
            _dbFilteredProducts= db.Products.AsQueryable().Where(x => x.Supplier.bizId == CurrentBusiness.Id);
            _dbFilteredPO = db.POes.AsQueryable().Where(x => x.Supplier.bizId == CurrentBusiness.Id);
            _dbFilteredPoD = db.PODs.AsQueryable().Where(x => x.Product.Supplier.bizId == CurrentBusiness.Id);
            _dbFilteredAccounts = db.Accounts.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
        }
        public POPRController()
        {   
            //Business CurrentBusiness = (Business)Session["CurrentBusiness"];
            //db.Products = (DbSet<Product>)db.Products.AsQueryable().Where(x => x.Supplier.Business.Id == CurrentBusiness.Id);
            //db.Suppliers = (DbSet<Supplier>)db.Suppliers.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
            //db.POes = (DbSet<PO>)db.POes.AsQueryable().Where(x => x.Supplier.bizId == CurrentBusiness.Id);
            //db.PODs = (DbSet<POD>)db.PODs.AsQueryable().Where(x=>x.Product.Supplier.bizId==CurrentBusiness.Id);
    }
       
        // GET: POes
        public ActionResult Index()
        {
            DateTime PKDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
            var dtStartDate = new DateTime(PKDate.Year, PKDate.Month, 1);
            var dtEndtDate = dtStartDate.AddMonths(1).AddSeconds(-1);

            //IQueryable<PO> pOes = db.POes.Include(s => s.Supplier);
            IQueryable<PO> pOes = _dbFilteredPO.Where(x => x.Date >= dtStartDate && x.Date <= dtEndtDate).Include(s => s.Supplier);
            //pOes.ForEachAsync(m => m.Id = Encryption.Encrypt(m.Id));
            //var pOes = db.POes.Where(s => s.SaleReturn == false);
            GetTotalBalance(ref pOes);
            Dictionary<decimal, decimal> LstMaxSerialNo = new Dictionary<decimal, decimal>();
            decimal thisSerial = 0;
            foreach (PO itm in pOes)
            {
                thisSerial = (decimal)itm.Supplier.POes.Max(x => x.POSerial);

                if (!LstMaxSerialNo.ContainsKey((decimal)itm.SupplierId))
                {
                    LstMaxSerialNo.Add(itm.Supplier.Id, thisSerial);
                }

                //itm.Id = Encryption.Encrypt(itm.Id);
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id)));
            }
            ViewBag.LstMaxSerialno = LstMaxSerialNo;
            ViewBag.Suppliers = _dbFilteredSuppliers;
            ViewBag.StartDate = dtStartDate.ToString("dd-MMM-yyyy");
            ViewBag.EndDate = dtEndtDate.ToString("dd-MMM-yyyy");
            return View(_dbFilteredPO.OrderByDescending(i => i.Date).ToList());
        }
        //public ActionResult SearchData(string custName, DateTime startDate, DateTime endDate)

        //public ActionResult SearchData(string custName, string startDate, string endDate)
        public ActionResult SearchData(string suppId, string startDate, string endDate)
        {

            int intSuppId;
            DateTime dtStartDate;
            DateTime dtEndtDate;
            IQueryable<PO> selectedPOes = null;
            if (endDate != string.Empty)
            {
                dtEndtDate = DateTime.Parse(endDate);
                dtEndtDate = dtEndtDate.AddDays(1);
                endDate = dtEndtDate.ToString();

            }

            if (suppId != string.Empty && startDate != string.Empty && endDate != string.Empty)
            {
                intSuppId = Int32.Parse(suppId);
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);

                selectedPOes = _dbFilteredPO.Where(so => so.SupplierId == intSuppId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            if (suppId == string.Empty && startDate == string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedPOes = _dbFilteredPO;//.Where(so => so.SupplierId == intSuppId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customers data acornding to start end date
            if (suppId == string.Empty && startDate != string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Parse(endDate);

                selectedPOes = _dbFilteredPO.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with from undefined startdate to this defined enddate
            if (suppId != string.Empty && startDate == string.Empty && endDate != string.Empty)
            {
                intSuppId = Int32.Parse(suppId.Trim());
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedPOes = _dbFilteredPO.Where(so => so.SupplierId == intSuppId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with from defined start date to undefined end date
            if (suppId != string.Empty && startDate != string.Empty && endDate == string.Empty)
            {
                intSuppId = Int32.Parse(suppId.Trim());
                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedPOes = _dbFilteredPO.Where(so => so.SupplierId == intSuppId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get this customer with all dates
            if (suppId != string.Empty && startDate == string.Empty && endDate == string.Empty)
            {
                intSuppId = Int32.Parse(suppId.Trim());
                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedPOes = _dbFilteredPO.Where(so => so.SupplierId == intSuppId && so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customer with defined startdate and undefined end date
            if (suppId == string.Empty && startDate != string.Empty && endDate == string.Empty)
            {

                dtStartDate = DateTime.Parse(startDate);
                dtEndtDate = DateTime.Today.AddDays(1);

                selectedPOes = _dbFilteredPO.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            //get all customers with undifined start date with defined enddate
            if (suppId == string.Empty && startDate == string.Empty && endDate != string.Empty)
            {

                dtStartDate = DateTime.Parse("1-1-1800");
                dtEndtDate = DateTime.Parse(endDate);

                selectedPOes = _dbFilteredPO.Where(so => so.Date >= dtStartDate && so.Date <= dtEndtDate);

            }

            GetTotalBalance(ref selectedPOes);
            Dictionary<decimal, decimal> LstMaxSerialNo = new Dictionary<decimal, decimal>();
            decimal thisSerial = 0;
            foreach (PO itm in selectedPOes)
            {
                thisSerial = (decimal)itm.Supplier.POes.Max(x => x.POSerial);
                if (!LstMaxSerialNo.ContainsKey((decimal)itm.SupplierId))
                {
                    LstMaxSerialNo.Add(itm.Supplier.Id, thisSerial);
                }
                //itm.Id = Encryption.Encrypt(itm.Id);
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id)));
            }

            return PartialView("_SelectedPOPR", selectedPOes.OrderByDescending(i => i.Date).ToList());

            //return View("Some thing went wrong");


        }
        public ActionResult PerMonthPurchase(int productId)
        {
            IQueryable<PO> pOes = _dbFilteredPO.Include(s => s.Supplier);

            //sOes = db.SOes.Where(x => x.SODs.Where(y => y.ProductId == productId));

            List<POD> lstPODs = _dbFilteredPoD.Where(x => x.ProductId == productId && x.SaleType == false).ToList();
            List<PO> lstSlectedPO = new List<PO>();
            foreach (POD lpod in lstPODs)
            {
                if (lstSlectedPO.Where(x => x.Id == lpod.POId).FirstOrDefault() == null)
                {
                    lstSlectedPO.Add(lpod.PO);
                }
            }

            pOes = lstSlectedPO.ToList().AsQueryable();
            foreach (PO itm in pOes)
            {
                //itm.Id = Encryption.Encrypt(itm.Id);
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id)));
            }
            ViewBag.ProductName = _dbFilteredProducts.FirstOrDefault(x => x.Id == productId).Name;
            return View("PerMonthPurchase", pOes.OrderBy(i => i.Date).ToList());
        }

        public ActionResult SearchProduct(int productId)
        {
            IQueryable<PO> pOes = _dbFilteredPO.Include(s => s.Supplier);

            List<POD> lstPODs = _dbFilteredPoD.Where(x => x.ProductId == productId).ToList();
            List<PO> lstSlectedPO = new List<PO>();
            foreach (POD lpod in lstPODs)
            {
                if (lstSlectedPO.Where(x => x.Id == lpod.POId).FirstOrDefault() == null)
                {
                    lstSlectedPO.Add(lpod.PO);
                }


            }

            pOes = lstSlectedPO.ToList().AsQueryable();

            //sOes.ForEachAsync(m => m.Id = Encryption.Encrypt(m.Id));
            //var sOes = db.SOes.Where(s => s.SaleReturn == false);
            GetTotalBalance(ref pOes);
            foreach (PO itm in pOes)
            {

                //itm.Id = Encryption.Encrypt(itm.Id);
                itm.Id = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(itm.Id)));
            }
            ViewBag.Suppliers = _dbFilteredSuppliers;
            return View("Index", pOes.OrderByDescending(i => i.Date).ToList());
        }

        private void GetTotalBalance(ref IQueryable<PO> POes)
        {
            //IQueryable<SO> DistSOes = SOes.Select(x => x.CustomerId).Distinct();
            IQueryable<PO> DistPOes = POes.GroupBy(x => x.SupplierId).Select(y => y.FirstOrDefault());

            decimal TotalBalance = 0;
            foreach (PO itm in DistPOes)
            {
                Supplier cust = db.Suppliers.Where(x => x.Id == itm.SupplierId).FirstOrDefault();
                TotalBalance += (decimal)cust.Balance;
            }
            ViewBag.TotalBalance = TotalBalance;

        }
        //[ChildActionOnly]
        //public PartialViewResult _SelectedPOPR()
        //{

        //    return PartialView(db.POes);
        //}
        
        // GET: POes/Details/5
        public ActionResult Details(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PO pO = db.POes.Find(id);
            if (pO == null)
            {
                return HttpNotFound();
            }
            return View(pO);
        }

        // GET: POes/Create
        public ActionResult Create(string IsReturn)
        {
           
            //ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name");
            //ViewBag.Products = db.Products;
            ViewBag.Accounts = _dbFilteredAccounts;
            //int maxId = db.Suppliers.Max(p => p.Id);
            decimal maxId = db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewSuppId = maxId;


            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();
            purchaseOrderViewModel.Suppliers = _dbFilteredSuppliers;
            purchaseOrderViewModel.Products = _dbFilteredProducts.Where(x => x.Saleable == true);
            ViewBag.IsReturn = IsReturn;
            return View(purchaseOrderViewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Prefix = "Supplier", Include = "Name,Address")] Supplier Supplier, [Bind(Prefix = "PurchaseOrder", Include = "BillAmount,Balance,PrevBalance,BillPaid,Discount,SupplierId,Remarks,Remarks2,PaymentMethod,PaymentDetail,PurchaseReturn,AccountId")] PO pO, [Bind(Prefix = "PurchaseOrderDetail", Include = "ProductId,Quantity,SaleType,PerPack,IsPack,PurchasePrice")] List<POD> pOD)

        {
            
            //PO pO = new PO();
            if (ModelState.IsValid)
            {
                
                Supplier supp = db.Suppliers.FirstOrDefault(x => x.Id == pO.SupplierId);
                if (supp == null)
                {//its means new customer
                    //pO.SupplierId = 10;
                    //int maxId = db.Suppliers.Max(p => p.Id);
                    decimal maxId = db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
                    maxId += 1;
                    Business CurrentBusiness = (Business)Session["CurrentBusiness"];
                    Supplier.Id = maxId;
                    Supplier.bizId = CurrentBusiness.Id;

                    Supplier.Balance = pO.Balance;
                    db.Suppliers.Add(Supplier);
                    //db.SaveChanges();
                }
                else
                {//its means old customer. old customer balance should be updated.
                    //Supplier.Id = (int)pO.SupplierId;
                    supp.Balance = pO.Balance;
                    db.Entry(supp).State = EntityState.Modified;
                    //db.SaveChanges();

                    //Payment payment = new Payment();
                    //payment = db.Payments.Find(orderId);
                    //payment.Status = true;
                    //db.Entry(payment).State = EntityState.Modified;
                    //db.SaveChanges();

                }

                ////////////////////////////////////////
                //int maxId = db.POes.Max(p => p.Auto);
                int maxId1 = (int)db.POes.DefaultIfEmpty().Max(p => p == null ? 0 : p.POSerial);
                maxId1 += 1;
                pO.POSerial = maxId1;
                //pO.Date = DateTime.Now;
                pO.Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));
                //pO.SaleReturn = false;
                pO.Id = System.Guid.NewGuid().ToString().ToUpper();
                pO.PurchaseOrderAmount = 0;
                pO.PurchaseReturnAmount = 0;
                pO.PurchaseOrderQty = 0;
                pO.PurchaseReturnQty = 0;
                Employee emp = (Employee)Session["CurrentUser"];
                pO.EmployeeId = emp.Id;
                db.POes.Add(pO);
                //db.SaveChanges();
                int sno = 0;
                if (pOD != null)
                {
                    //pOD.RemoveAll(so => so.ProductId == null);
                    foreach (POD pod in pOD)
                    {
                        sno += 1;
                        pod.PODId = sno;
                        pod.PO = pO;
                        pod.POId = pO.Id;

                        Product product = db.Products.FirstOrDefault(x => x.Id == pod.ProductId);

                        //dont do this. when user enter manual purchase price that will not happen. it does not reflect in bill and calculations geting wrong
                        //pod.PurchasePrice = product.PurchasePrice;
                        if (pod.Quantity == null) { pod.Quantity = 0; }
                        pod.OpeningStock = product.Stock;
                        if (pod.SaleType == true)//return
                        {
                            
                            if (pod.IsPack == false)
                            {
                                pO.PurchaseReturnAmount += (decimal)(pod.Quantity * pod.PurchasePrice);
                                decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                                product.Stock -= qty;
                                pO.PurchaseReturnQty += qty;//(int)sod.Quantity;
                                
                            }
                            else
                            {
                                pO.PurchaseReturnAmount += (decimal)(pod.Quantity * pod.PurchasePrice * pod.PerPack);
                                product.Stock -= (int)pod.Quantity * pod.PerPack;

                                pO.PurchaseReturnQty += (int)pod.Quantity * pod.PerPack;
                                
                            }
                        }
                        else//purchase
                        {
                            
                            if (pod.IsPack == false)
                            {//piece
                                pO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice);
                                //int pieceSold = (int)(sod.Quantity * product.Stock);
                                decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                                product.Stock += qty;

                                pO.PurchaseOrderQty += qty;//(int)sod.Quantity;
                                
                            }
                            else
                            {//pack

                                pO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice * pod.PerPack);
                                product.Stock += (int)pod.Quantity * pod.PerPack;

                                pO.PurchaseOrderQty += (int)pod.Quantity * pod.PerPack;

                            }

                        }

                    }
                    db.PODs.AddRange(pOD);
                }
                string POId;
                ////////////////////////////v deffer the duplicate error
                int previousbillNum = maxId1 - 1;
                PO previousBill = db.POes.Where(x => x.POSerial == previousbillNum).FirstOrDefault();

                DateTime PrvTime = (DateTime)previousBill.Date;//previous bill time
                DateTime thisTime = (DateTime)pO.Date;//this bill time
                TimeSpan span = thisTime.Subtract(PrvTime);

                if (previousBill.BillAmount == pO.BillAmount && previousBill.BillPaid == pO.BillPaid
                    && previousBill.SupplierId == pO.SupplierId && previousBill.Discount == pO.Discount
                    && previousBill.PurchaseOrderAmount == pO.PurchaseOrderAmount && previousBill.PurchaseOrderQty == pO.PurchaseOrderQty
                    && previousBill.PurchaseReturnAmount == pO.PurchaseReturnAmount && previousBill.PurchaseReturnQty == pO.PurchaseReturnQty)
                //&& span.TotalSeconds < 60)
                {
                    //Duplicate bill found. don't do anything
                    POId = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(previousBill.Id)));

                    string path = Server.MapPath("~/log.txt");
                    System.IO.File.AppendAllText(path, DateTime.Now.ToString("dd/MM/yy hh:mm tt") + "\tPO" + System.Environment.NewLine);
                }
                else
                {
                    db.SaveChanges();
                    POId = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(pO.Id)));
                }
                ViewBag.POIDEnc = POId;
                ViewBag.IsReturn = false;

                ////////////////////////////^ deffer the duplicate error


                //SqlParameter param1 = new SqlParameter("@PurchaseOrderID", pO.Id);
                ////var result = db.Database.ExecuteSqlCommand("spSOReceipt @PurchaseOrderID", param1);
                //var result = db.Database.SqlQuery<Object>("spSOReceipt @PurchaseOrderID", param1);


                //var cr = new ReportDocument();
                //cr.Load(@"E:\PROJECTS\MYBUSINESS - v.4.6\MYBUSINESS\Reports\SOReceipt.rpt");
                //cr.DataDefinition.RecordSelectionFormula = "{PurchaseOrderID} = '" + pO.Id + "'";
                //cr.PrintToPrinter(1, true, 0, 0);


                ////////////////////////finalized
                //string pathh = HttpRuntime.AppDomainAppPath;
                //ReportDocument reportDocument = new ReportDocument();
                //reportDocument.Load(pathh + @"Reports\SOReceipt.rpt");
                //reportDocument.SetParameterValue("@PurchaseOrderID", pO.Id);
                //System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();
                ////printerSettings.PrinterName = PrinterName;
                //reportDocument.PrintToPrinter(printerSettings, new PageSettings(), false);
                /////////////////////////////////////


                //string POId = string.Join("-", ASCIIEncoding.ASCII.GetBytes(Encryption.Encrypt(pO.Id)));
                //return PrintSO(POId);
                return PrintSO3(POId);

                //return RedirectToAction("Index");
            }

            //ViewBag.SupplierId = new SelectList(_dbFilteredSuppliers, "Id", "Name", pO.SupplierId);
            //return View(pO);
            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();
            purchaseOrderViewModel.Suppliers = _dbFilteredSuppliers;
            purchaseOrderViewModel.Products = _dbFilteredProducts;

            return View(purchaseOrderViewModel);
            //return View();

        }
        //public void PrintSO(string POId)
        //{
        //    POId = Encryption.Decrypt(POId);
        //    string pathh = HttpRuntime.AppDomainAppPath;
        //    ReportDocument reportDocument = new ReportDocument();
        //    reportDocument.Load(pathh + @"Reports\POPRReceipt2.rpt");
        //    //reportDocument.SetDatabaseLogon("sa", "abc", "LAPTOP-MGR35B58", "Business");


        //    ////
        //    CrystalDecisions.CrystalReports.Engine.Database oCRDb = reportDocument.Database;
        //    CrystalDecisions.CrystalReports.Engine.Tables oCRTables = oCRDb.Tables;
        //    //CrystalDecisions.CrystalReports.Engine.Table oCRTable;
        //    CrystalDecisions.Shared.TableLogOnInfo oCRTableLogonInfo;
        //    CrystalDecisions.Shared.ConnectionInfo oCRConnectionInfo = new CrystalDecisions.Shared.ConnectionInfo();
        //    oCRConnectionInfo.DatabaseName = "Business";
        //    oCRConnectionInfo.ServerName = "(local)";
        //    oCRConnectionInfo.UserID = "sa";
        //    oCRConnectionInfo.Password = "abc";
        //    foreach (CrystalDecisions.CrystalReports.Engine.Table oCRTable in oCRTables)
        //    {
        //        oCRTableLogonInfo = oCRTable.LogOnInfo;
        //        oCRTableLogonInfo.ConnectionInfo = oCRConnectionInfo;
        //        oCRTable.ApplyLogOnInfo(oCRTableLogonInfo);
        //    }
        //    ////

        //    reportDocument.SetParameterValue("@PurchaseOrderID", POId);
        //    System.Drawing.Printing.PrinterSettings printerSettings = new System.Drawing.Printing.PrinterSettings();
        //    //printerSettings.PrinterName = "abc";
        //    reportDocument.PrintToPrinter(printerSettings, new PageSettings(), false);

        //}
    

        public FileContentResult PrintSO2(string id)
        {
            id = Decode(id);
            LocalReport localreport = new LocalReport();
            localreport.ReportPath = Server.MapPath("~/Reports/Report3.rdlc");
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "ReportDataSet";
            reportDataSource.Value = null;//db.vSaleOrders.Where(x=> x.Id==id);
            localreport.DataSources.Add(reportDataSource);
            string reportType = "PDF";
            string mimeType;
            string encoding;
            string fileNameExtension = "pdf";
            Warning[] warnings;
            string[] streams;
            byte[] renderBytes;

            renderBytes = localreport.Render(reportType, null, out mimeType, out encoding, out fileNameExtension, out streams, out warnings);
            Response.AddHeader("content-disposition", "attachment; filename=Urls." + fileNameExtension);

            return File(renderBytes, mimeType);

        }
        public FileContentResult PrintSO3(string id)
        {


            id = Decode(id);
            // Variables
            Warning[] warnings;
            string[] streamIds;
            string mimeType = "application/pdf";
            string encoding = string.Empty;
            string extension = "pdf";


            // Setup the report viewer object and get the array of bytes
            ReportViewer viewer = new ReportViewer();
            viewer.ProcessingMode = ProcessingMode.Local;

            //PO pO = db.POes.FirstOrDefault(x => x.Id == id);
            Employee emp = db.Employees.FirstOrDefault(x => x.Id == db.POes.FirstOrDefault(y => y.Id == id).EmployeeId);
            if (emp.Login == "LahoreKarachi")
            { viewer.LocalReport.ReportPath = Server.MapPath("~/Reports/Purchase_LahoreKarachi.rdlc"); }
            else
            { viewer.LocalReport.ReportPath = Server.MapPath("~/Reports/Purchase_GoldPanel.rdlc"); }

            ReportDataSource reportDataSource = new ReportDataSource();

            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = db.spPOReport(id).AsEnumerable();//db.spSOReceipt;// BusinessDataSetTableAdapters
            viewer.LocalReport.DataSources.Add(reportDataSource);
            viewer.LocalReport.SetParameters(new ReportParameter("PurchaseOrderID", id));
            viewer.LocalReport.Refresh();
            //byte[] bytes = viewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamIds, out warnings);
            byte[] bytes = viewer.LocalReport.Render("PDF");//, null, out mimeType, out encoding, out extension, out streamIds, out warnings);
            // Now that you have all the bytes representing the PDF report, buffer it and send it to the client.
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.AddHeader("content-disposition", "inline;filename=" + "myfile" + "." + extension);
            Response.BinaryWrite(bytes); // create the file
            //return Response.Flush(); // send it to the client to download
            return new FileContentResult(bytes, mimeType);




            //System.IO.Stream stream = rd.ExportToStream(CrystalDecisions.Shared.ExportFormatType.PortableDocFormat);

            ////stream.Seek(0, System.IO.SeekOrigin.Begin);
            //System.IO.BinaryReader br = new System.IO.BinaryReader(stream);
            //byte[] getBytes = null;
            //getBytes = br.ReadBytes(Convert.ToInt32(br.BaseStream.Length));
            //HttpContext.Response.AddHeader("content-disposition", "inline; filename=" + "POPR.pdf");

            //return File(getBytes, "application/pdf");









        }

        public decimal GetPreviousBalance(int id)
        {
            IQueryable lstSO = db.POes.Where(x => x.SupplierId == id);

            //lstSO.ForEachAsync(c => { c. = 0; c.GroupID = 0; c.CompanyID = 0; });
            decimal POAmount = 0;
            decimal PRAmount = 0;
            foreach (PO itm in lstSO)
            {
                POAmount += (decimal)itm.PurchaseOrderAmount;
                PRAmount += (decimal)itm.PurchaseReturnAmount;

            }

            return (POAmount - PRAmount);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Prefix = "PurchaseOrder", Include = "BillAmount,Balance,BillPaid,Discount")] PO pO, [Bind(Prefix = "PurchaseOrderDetail", Include = "ProductId,Quantity")] List<POD> pOD)
        public ActionResult Edit(PurchaseOrderViewModel purchaseOrderViewModel1)
        {
            PO newPO = purchaseOrderViewModel1.PurchaseOrder;
            List<POD> newPODs = purchaseOrderViewModel1.PurchaseOrderDetail;
            if (ModelState.IsValid)
            {
                newPO.Id = Encryption.Decrypt(purchaseOrderViewModel1.PurchaseOrder.Id);//
                PO PO = db.POes.Where(x => x.Id == newPO.Id).FirstOrDefault();
                PO.Date = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Pakistan Standard Time"));//
                //PO.PurchaseReturn = false;//
                PO.BillAmount = newPO.BillAmount;//
                PO.Discount = newPO.Discount;//
                PO.BillPaid = newPO.BillPaid;//
                PO.Balance = newPO.Balance;//
                PO.Remarks = newPO.Remarks;//
                PO.Remarks2 = newPO.Remarks;//
                PO.PaymentMethod = newPO.PaymentMethod;
                PO.PaymentDetail = newPO.PaymentDetail;

                //PO.POSerial = newPO.POSerial;//should be unchanged

                ///////////////////////////////////////////

                //Supplier cust = db.Suppliers.FirstOrDefault(x => x.Id == newPO.SupplierId);
                Supplier supplier = db.Suppliers.Where(x => x.Id == newPO.SupplierId).FirstOrDefault();
                if (supplier == null)
                {//its means new supplier(not in db)
                 //PO.SupplierId = 10;
                 //int maxId = db.Suppliers.Max(p => p.Id);
                    supplier = purchaseOrderViewModel1.Supplier;
                    decimal maxId = db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
                    maxId += 1;

                    supplier.Id = maxId;
                    //supplier.Balance = newPO.Balance;
                    db.Suppliers.Add(supplier);
                }
                else
                {
                    db.Entry(supplier).State = EntityState.Modified;
                }

                if (PO.SupplierId != newPO.SupplierId)
                {//POme other db supplier
                 //first revert the previous supplier balance 
                    Supplier oldSupplier = db.Suppliers.Where(x => x.Id == PO.SupplierId).FirstOrDefault();
                    oldSupplier.Balance = _dbFilteredPO.Where(x => x.Id == PO.Id).FirstOrDefault().PrevBalance;
                    db.Entry(oldSupplier).State = EntityState.Modified;
                }

                PO.PrevBalance = newPO.PrevBalance;//
                // assign balance of this supplier
                //Supplier supplier = db.Suppliers.Where(x => x.Id == newPO.SupplierId).FirstOrDefault();
                supplier.Balance = newPO.Balance;
                //assign supplier and supplierId in PO
                PO.SupplierId = newPO.SupplierId;
                PO.Supplier = supplier;

                /////////////////////////////////////////////////////////////////////////////



                List<POD> oldPODs = db.PODs.Where(x => x.POId == newPO.Id).ToList();

                //handling old prodcts quantity. add old quantites back to the stock, then in next loop product quantity will be minus. this is simple and stateforward.
                foreach (POD pod in oldPODs)
                {
                    Product product = db.Products.FirstOrDefault(x => x.Id == pod.ProductId);
                    if (pod.SaleType == false)//purchase
                    {
                        //product.Stock -= pod.Quantity;

                        if (pod.IsPack == false)
                        {
                            decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                            product.Stock -= qty;
                        }
                        else
                        {
                            product.Stock -= (int)pod.Quantity * pod.PerPack;
                        }

                    }
                    else//return
                    {
                        //product.Stock += pod.Quantity;

                        if (pod.IsPack == false)
                        {
                            decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                            product.Stock += qty;
                        }
                        else
                        {
                            product.Stock += (int)pod.Quantity * pod.PerPack;
                        }


                    }
                    db.Entry(product).State = EntityState.Modified;
                }

                db.PODs.RemoveRange(oldPODs);
                //////////////////////////////////////////////////////////////////////////////

                PO.PurchaseOrderAmount = 0;
                PO.PurchaseReturnAmount = 0;
                PO.PurchaseOrderQty = 0;
                PO.PurchaseReturnQty = 0;
                //PO.Profit = 0;
                int sno = 0;

                if (newPODs != null)
                {

                    foreach (POD pod in newPODs)
                    {
                        sno += 1;
                        pod.PODId = sno;
                        pod.PO = PO;
                        pod.POId = PO.Id;

                        Product product = db.Products.FirstOrDefault(x => x.Id == pod.ProductId);
                        //POd.purchasePrice is now from view
                        //POd.PurchasePrice = product.PurchasePrice;
                        //dont do this. calculation are geting wrong. when user open an old bill and just press save. all calculations distrubs
                        //pod.PurchasePrice = product.PurchasePrice;
                        if (pod.Quantity == null) { pod.Quantity = 0; }
                        pod.OpeningStock = product.Stock;
                        if (pod.SaleType == true)//return
                        {
                            if (pod.IsPack == false)
                            {
                                PO.PurchaseReturnAmount += (decimal)(pod.Quantity * pod.PurchasePrice);
                                decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                                product.Stock -= qty;
                                PO.PurchaseReturnQty += qty;//(int)sod.Quantity;

                            }
                            else
                            {
                                PO.PurchaseReturnAmount += (decimal)(pod.Quantity * pod.PurchasePrice * pod.PerPack);
                                product.Stock -= (int)pod.Quantity * pod.PerPack;

                                PO.PurchaseReturnQty += (int)pod.Quantity *pod.PerPack;

                            }
                        }
                        else//purchase
                        {
                            if (pod.IsPack == false)
                            {//piece
                                PO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice);
                                //int pieceSold = (int)(sod.Quantity * product.Stock);
                                decimal qty = (decimal)pod.Quantity;// / (decimal)product.PerPack;
                                product.Stock += qty;

                                PO.PurchaseOrderQty += qty;//(int)sod.Quantity;

                            }
                            else
                            {//pack

                                PO.PurchaseOrderAmount += (decimal)(pod.Quantity * pod.PurchasePrice * pod.PerPack);
                                product.Stock += (int)pod.Quantity * pod.PerPack;

                                PO.PurchaseOrderQty += (int)pod.Quantity * pod.PerPack;

                            }
                        }

                    }
                    //PO.Profit -= (decimal)PO.Discount;
                    db.Entry(PO).State = EntityState.Modified;
                    db.Entry(PO).Property(x => x.POSerial).IsModified = false;
                    db.PODs.AddRange(newPODs);

                }
                db.SaveChanges();

                return RedirectToAction("Index");
            }

            //ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name", PO.SupplierId);
            //return View(PO);
            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();

            purchaseOrderViewModel.Products = db.Products;
            return View(purchaseOrderViewModel);
            //return View();
        }
        private string Decode(string id)
        {
            byte[] BytesArr = id.Split('-').Select(byte.Parse).ToArray();
            id = new string(Encoding.UTF8.GetString(BytesArr).ToCharArray());
            id = Encryption.Decrypt(id);
            return id;
        }




        // GET: POes/Edit/5
        public ActionResult Edit(string id, bool update)
        {

            if (id == null)
            {

                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            //byte[] BytesArr = id.Split('-').Select(byte.Parse).ToArray();
            //id = new string( Encoding.UTF8.GetString(BytesArr).ToCharArray());
            //id = Encryption.Decrypt(id,"BZNS");

            decimal maxId = db.Suppliers.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedNewSuppId = maxId;

            List<MySaleType> myOptionLst = new List<MySaleType> {
                            new MySaleType {
                                Text = "Order",
                                Value = "false"
                            },
                            new MySaleType {
                                Text = "Return",
                                Value = "true"
                            }
                        };
            ViewBag.OptionLst = myOptionLst;

            ////////////////
            List<MyPaymentMethod> myPaymentOptionLst = new List<MyPaymentMethod> {
                            new MyPaymentMethod {
                                Text = "Cash",
                                Value = "Cash"
                            },
                            new MyPaymentMethod {
                                Text = "Online",
                                Value = "Online"
                            },
                            new MyPaymentMethod {
                                Text = "Cheque",
                                Value = "Cheque"
                            },
                            new MyPaymentMethod {
                                Text = "Other",
                                Value = "Other"
                            }
                        };

            ViewBag.PaymentMethodOptionLst = myPaymentOptionLst;

            List<MyUnitType> myUnitTypeOptionList = new List<MyUnitType> {
                            new MyUnitType {
                                Text = "Piece",
                                Value = "false"
                            },
                            new MyUnitType {
                                Text = "Pack",
                                Value = "true"
                            }
                        };

            ViewBag.UnitTypeOptionList = myUnitTypeOptionList;
            string iid = Decode(id);
            Payment pmnt = db.Payments.Where(x => x.SOId == iid).FirstOrDefault();
            if (pmnt != null)
            {
                ViewBag.paymentMethod = pmnt.PaymentMethod;
                ViewBag.paymentRemarks = pmnt.Remarks;
            }
            ///////////////////

            id = Decode(id);

            PO pO = db.POes.Find(id);
            if (pO == null)
            {
                return HttpNotFound();
            }
            PurchaseOrderViewModel purchaseOrderViewModel = new PurchaseOrderViewModel();
            List<POD> pod = db.PODs.Where(x => x.POId == id).ToList();
            purchaseOrderViewModel.Products = _dbFilteredProducts;
            purchaseOrderViewModel.Suppliers = _dbFilteredSuppliers;
            purchaseOrderViewModel.PurchaseOrderDetail = pod;
            pO.Id = Encryption.Encrypt(pO.Id);
            purchaseOrderViewModel.PurchaseOrder = pO;
            int orderQty = 0;
            int orderQtyPiece = 0;//orderQtyPiece 'P for piece' 
            int returnQty = 0;
            int returnQtyPiece = 0;//orderQtyPiece 'P for piece' 
            foreach (var item in pod)
            {
                if (pO.PurchaseReturn == false)
                {
                    if (item.IsPack == true)
                    {//Pack
                        orderQty += (int)item.Quantity;
                    }
                    else
                    {//Item
                        orderQtyPiece += (int)item.Quantity;
                    }
                }
                else
                {
                    if (item.IsPack == true)
                    {//Pack
                        returnQty += (int)item.Quantity;
                    }
                    else
                    {//Item
                        returnQtyPiece += (int)item.Quantity;
                    }

                }

            }
            ViewBag.orderQty = orderQty;
            ViewBag.orderQtyPiece = orderQtyPiece;
            ViewBag.returnQty = returnQty;
            ViewBag.returnQtyPiece = returnQtyPiece;
            //ViewBag.SupplierId = new SelectList(db.Suppliers, "Id", "Name", sO.SupplierId);
            ViewBag.SupplierName = pO.Supplier.Name;
            ViewBag.SupplierAddress = pO.Supplier.Address;
            decimal subTotal = (decimal)(pO.PurchaseOrderAmount - pO.PurchaseReturnAmount - pO.Discount);
            ViewBag.SubTotal = subTotal;
            ViewBag.Total = subTotal + (decimal)pO.PrevBalance;
            ViewBag.IsUpdate = update;
            ViewBag.IsReturn = pO.PurchaseReturn.ToString().ToLower();
            return View(purchaseOrderViewModel);
        }


        // GET: POes/Delete/5
        public ActionResult Delete(string id)
        {
            return null;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            id = Decode(id);
            PO pO = db.POes.Find(id);
            if (pO == null)
            {
                return HttpNotFound();
            }
            return View(pO);
        }

        // POST: POes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            return null;
            id = Decode(id);

            List<POD> oldSODs = db.PODs.Where(x => x.POId == id).ToList();
            //handling old prodcts quantity. add old quantites back to the stock, then in next loop product quantity will be minus. this si simple and stateforward.
            foreach (POD pod in oldSODs)
            {
                Product product = db.Products.FirstOrDefault(x => x.Id == pod.ProductId);
                product.Stock += pod.Quantity;
            }
            db.PODs.RemoveRange(oldSODs);

            PO pO = db.POes.Find(id);
            db.POes.Remove(pO);
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
