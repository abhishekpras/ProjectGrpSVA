using LINQtoCSV;
using ProjectGrpSVA.DataAccess;
using ProjectGrpSVA.DataAccess.DL;
using ProjectGrpSVA.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProjectGrpSVA.Controllers
{
    public class HomeController : Controller
    {
        DataModel db = new DataModel();

        // GET: Home 
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UploadFiles()
        {
            return View(db.Aisle.OrderBy(e => e.aisle_id));
        }
        [HttpPost]
        public ActionResult uploadAsileCsv(HttpPostedFileBase attachmentcsv)
        {
            CsvFileDescription csvFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true
            };
            CsvContext csvContext = new CsvContext();
            StreamReader streamReader = new StreamReader(attachmentcsv.InputStream);
            IEnumerable<Aisle> list = csvContext.Read<Aisle>(streamReader, csvFileDescription);
            db.Aisle.AddRange(list);
            db.SaveChanges();
            return Redirect("UploadFiles");
        }

        public ActionResult DepartmentFiles()
        {
            return View(db.Departments.OrderBy(e => e.department_id));
        }
        [HttpPost]
        public ActionResult uploadDepartCsv(HttpPostedFileBase attachmentcsv)
        {
            CsvFileDescription csvFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true
            };
            CsvContext csvContext = new CsvContext();
            StreamReader streamReader = new StreamReader(attachmentcsv.InputStream);
            IEnumerable<Departments> list = csvContext.Read<Departments>(streamReader, csvFileDescription);
            db.Departments.AddRange(list);
            db.SaveChanges();
            return Redirect("DepartmentFiles");
        }

        public ActionResult ProductFiles()
        {
            var query =
            (from p in db.Product
             join o in db.Aisle on p.aisle_id equals o.aisle_id
             join d in db.Departments on p.department_id equals d.department_id
             select new
             {
                 Product_Id = p.product_id,
                 Product_Name = p.product_name,
                 Aisle_Nme = o.aisle,
                 Department_Name = d.department
             }).ToList();

            List<ProductDisplay> productList = new List<ProductDisplay>();
            foreach (var prd in query)
            {
                ProductDisplay prdDis = new ProductDisplay();
                prdDis.Product_Id = prd.Product_Id;
                prdDis.Product_Name = prd.Product_Name;
                prdDis.Aisle_Nme = prd.Aisle_Nme;
                prdDis.Department_Name = prd.Department_Name;
                productList.Add(prdDis);
            }

            return View(productList);
        }
        [HttpPost]
        public ActionResult uploadProductCsv(HttpPostedFileBase attachmentcsv)
        {
            CsvFileDescription csvFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true
            };
            CsvContext csvContext = new CsvContext();
            StreamReader streamReader = new StreamReader(attachmentcsv.InputStream);
            IEnumerable<Product> list = csvContext.Read<Product>(streamReader, csvFileDescription);
            db.Product.AddRange(list);
            db.SaveChanges();
            return Redirect("ProductFiles");
        }
    }
}