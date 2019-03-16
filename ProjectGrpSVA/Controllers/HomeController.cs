using LINQtoCSV;
using Newtonsoft.Json;
using ProjectGrpSVA.DataAccess;
using ProjectGrpSVA.DataAccess.DL;
using ProjectGrpSVA.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        // GET: Home 
        public ActionResult Analytics()
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

        public ActionResult OrderProductFiles()
        {
            var query =
                (from p in db.Order_Products
                 join o in db.Product on p.product_id equals o.product_id
                 select new
                 {
                     Order_Id = p.product_id,
                     Product_Name = o.product_name,
                     Add_To_Cart_Order = p.add_to_cart_order,
                     Reordered = p.product_id
                 }).ToList();

            List<OrderDisplay> orderList = new List<OrderDisplay>();
            foreach (var ord in query)
            {
                OrderDisplay ordDis = new OrderDisplay();
                ordDis.Order_Id = ord.Order_Id;
                ordDis.Product_Name = ord.Product_Name;
                ordDis.Add_To_Cart_Order = ord.Add_To_Cart_Order;
                ordDis.Reordered = ord.Reordered;
                orderList.Add(ordDis);
            }
            return View(orderList);
        }
        [HttpPost]
        public ActionResult uploadOrderProductCsv(HttpPostedFileBase attachmentcsv)
        {
            CsvFileDescription csvFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true
            };
            CsvContext csvContext = new CsvContext();
            StreamReader streamReader = new StreamReader(attachmentcsv.InputStream);
            IEnumerable<Order_Products> list = csvContext.Read<Order_Products>(streamReader, csvFileDescription);
            db.Order_Products.AddRange(list);
            db.SaveChanges();
            return Redirect("OrderProductFiles");
        }

        public ActionResult OrderFiles()
        {
             return View(db.Orders.OrderBy(e => e.order_id));
        }
        [HttpPost]
        public ActionResult uploadOrderCsv(HttpPostedFileBase attachmentcsv)
        {
            CsvFileDescription csvFileDescription = new CsvFileDescription
            {
                SeparatorChar = ',',
                FirstLineHasColumnNames = true
            };
            CsvContext csvContext = new CsvContext();
            StreamReader streamReader = new StreamReader(attachmentcsv.InputStream);
            IEnumerable<Orders> list = csvContext.Read<Orders>(streamReader, csvFileDescription);
            db.Orders.AddRange(list);
            db.SaveChanges();
            return Redirect("OrderFiles");
        }

        public ActionResult HourOfDay()
        {
            var query = db.Orders
                   .GroupBy(p => p.order_hour_of_day)
                   .Select(g => new { name = g.Key, count = g.Count() });

            string json = JsonConvert.SerializeObject(query);

            ViewBag.json = json;

            return View();
        }

        public ActionResult DayOfWeek()
        {
            var query = db.Orders
                   .GroupBy(p => p.order_dow)
                   .Select(g => new { name = g.Key, count = g.Count() });

            string json = JsonConvert.SerializeObject(query);

            ViewBag.json = json;

            return View();
        }

        public ActionResult DaySincePriorOrder()
        {
            var query = db.Orders
                   .GroupBy(p => p.days_since_prior_order)
                   .Select(g => new { name = g.Key, count = g.Count() });
           

            string json = JsonConvert.SerializeObject(query);

            ViewBag.json = json;

            return View();
        }

        public ActionResult PriorOrders()
        {
            var query = db.Orders.Where(u => u.eval_set == "prior");

            string json = JsonConvert.SerializeObject(query);

            ViewBag.json = json;

            return View();
        }

        public ActionResult ItemsDoPeopleBuy()
        {
            var query = db.Order_Products
                   .GroupBy(p => p.order_id)
                   .Select(g => new { name = g.Key, count = g.Count() });

            string json = JsonConvert.SerializeObject(query);

            ViewBag.json = json;

            return View();
        }

        public ActionResult MarketBasketAnalysis()
        {
            return View();
        }

        public ActionResult IncomePrediction()
        {
            return View();
        }

        public ActionResult NaturalLanguageProcessing()
        {
            return View();
        }

        public ActionResult Clustering()
        {
            return View();
        }

        public ActionResult MoreAnalytics()
        {
            return View();
        }



    }
}