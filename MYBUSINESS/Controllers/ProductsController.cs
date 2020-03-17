using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using MYBUSINESS.Models;

namespace MYBUSINESS.Controllers
{
    public class ProductsController : Controller
    {
        private BusinessContext db = new BusinessContext();
        private IQueryable<Product> _dbFilteredProducts;
        private IQueryable<Supplier> _dbFilteredSuppliers;
        public ProductsController()
        {
            //Business CurrentBusiness = (Business)Session["CurrentBusiness"];
            //db.Products = (DbSet<Product>)db.Products.AsQueryable().Where(x => x.Supplier.Business.Id == CurrentBusiness.Id);
            //db.Suppliers = (DbSet<Supplier>)db.Suppliers.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
        }
        protected override void Initialize(RequestContext requestContext)
        {

            base.Initialize(requestContext);

            Business CurrentBusiness = (Business)Session["CurrentBusiness"];
            _dbFilteredProducts = db.Products.AsQueryable().Where(x => x.Supplier.bizId == CurrentBusiness.Id);
            _dbFilteredSuppliers = db.Suppliers.AsQueryable().Where(x => x.bizId == CurrentBusiness.Id);
        }
        // GET: Products
        public ActionResult Index()
        {
            ViewBag.Suppliers = _dbFilteredSuppliers;
            return View(_dbFilteredProducts.ToList());
        }

        public ActionResult SearchData(string suppId)
        {
            if (suppId.Trim() == string.Empty)
            {

                return PartialView("_SelectedProducts", db.Products.OrderBy(i => i.Id).ToList());

            }
            else
            {
                int intSuppId = Int32.Parse(suppId.Trim());

                IQueryable<Product> selectedProducts = null;
                selectedProducts = db.Products.Where(p => p.SupplierId == intSuppId);
                return PartialView("_SelectedProducts", selectedProducts.OrderBy(i => i.Id).ToList());

            }

        }

        //// GET: Products/Details/5
        //public ActionResult Details(decimal id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Product product = db.Products.Find(id);
        //    if (product == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(product);
        //}

        // GET: Products/Create
        public ActionResult Create()
        {
            //int maxId = db.Products.Max(p => p.Id);
            decimal maxId = db.Products.DefaultIfEmpty().Max(p => p == null ? 0 : p.Id);
            maxId += 1;
            ViewBag.SuggestedId = maxId;
            ViewBag.Suppliers = _dbFilteredSuppliers;
            Product prod = new Product();

            prod.PurchasePrice = 0;
            prod.SalePrice = 0;
            prod.Stock = 0;

            prod.Saleable = true;
            return View(prod);
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,PurchasePrice,SalePrice,Stock,SupplierId,Saleable,PerPack,ImgPath")] Product product,String AddAnother)
        {
            if (product.Stock == null)
            {
                product.Stock = 0;
            }

            if (product.PerPack == null || product.PerPack == 0)
            {
                product.PerPack = 1;
            }

            product.Stock = product.Stock * product.PerPack;
            
            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase file = Request.Files[0];
                    if (file.ContentLength > 0)
                    {
                        String FileName = Path.GetFileNameWithoutExtension(file.FileName);

                        FileName = "Product" + product.Id;
                        string Extention = Path.GetExtension(file.FileName);
                        FileName = FileName + Extention;
                        product.ImgPath = "~/Image/" + FileName;
                        FileName = Path.Combine(Server.MapPath("~/Image/"), FileName);
                        file.SaveAs(FileName);
                       

                    }
                }

                db.Products.Add(product);
                db.SaveChanges();

                if (!string.IsNullOrEmpty(AddAnother))

                {
                    return RedirectToAction("Create");
                }
                else { return RedirectToAction("Index"); }
            }
            ViewBag.Suppliers = _dbFilteredSuppliers;
            return View(product);
        }

        // GET: Products/Edit/5
        public ActionResult Edit(decimal id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Product product = db.Products.Find(id);
            product.Stock = product.Stock / product.PerPack;
            ViewBag.SuppName = product.Supplier.Name;
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,PurchasePrice,SalePrice,Stock,Saleable,SupplierId,PerPack")] Product product)
        {
            //Product prd = db.Products.Where(x => x.Id == product.Id).FirstOrDefault();
            //product.SuppId = prd.SuppId;
            if (product.Stock == null)
            {
                product.Stock = 0;
            }

            if (product.PerPack == null || product.PerPack == 0)
            {
                product.PerPack = 1;
            }

            product.Stock = product.Stock * product.PerPack;


            if (ModelState.IsValid)
            {

                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(product);
        }

        //// GET: Products/Delete/5
        //public ActionResult Delete(decimal id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Product product = db.Products.Find(id);
        //    if (product == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(product);
        //}

        //// POST: Products/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(decimal id)
        //{
        //    Product product = db.Products.Find(id);
        //    db.Products.Remove(product);
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
