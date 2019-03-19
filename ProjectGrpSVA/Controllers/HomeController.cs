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
        var list = db.Orders.OrderBy(p => p.order_hour_of_day).Select(p => new { p.order_hour_of_day }).ToList();
            List<int> repartitions = new List<int>();
            var hod_List = list.Select(x => x.order_hour_of_day).Distinct();
            
            foreach(var item in hod_List)
            {
                repartitions.Add(list.Count(x => x.order_hour_of_day == item));
            }

            var rep = repartitions;
            ViewBag.hod = hod_List;
            ViewBag.rep = repartitions.ToList();

             return View();
        }

        public ActionResult DayOfWeek()
        {
            var list = db.Orders.OrderBy(p => p.order_dow).Select(p => new { p.order_dow }).ToList();
            List<int> repartitions = new List<int>();
            var hod_List = list.Select(x => x.order_dow).Distinct();

            foreach (var item in hod_List)
            {
                repartitions.Add(list.Count(x => x.order_dow == item));
            }

            var rep = repartitions;
            ViewBag.hod = hod_List;
            ViewBag.rep = repartitions.ToList();

            return View();

        }

        public ActionResult DaySincePriorOrder()
        {
            var list = db.Orders.OrderBy(p => p.days_since_prior_order).Select(p => new { p.days_since_prior_order }).ToList();
            List<int> repartitions = new List<int>();
            var hod_List = list.Select(x => x.days_since_prior_order).Distinct();

            foreach (var item in hod_List)
            {
                repartitions.Add(list.Count(x => x.days_since_prior_order == item));
            }

            var rep = repartitions;
            ViewBag.hod = hod_List;
            ViewBag.rep = repartitions.ToList();

            return View();
        }

        public ActionResult ItemsDoPeopleBuy()
        {
            var list = db.Order_Products.OrderBy(p => p.order_id).Select(p => new { p.order_id }).ToList();
            List<int> repartitions = new List<int>();
            var hod_List = list.Select(x => x.order_id).Distinct();

            foreach (var item in hod_List)
            {
                repartitions.Add(list.Count(x => x.order_id == item));
            }

            var rep = repartitions;
            ViewBag.hod = hod_List;
            ViewBag.rep = repartitions.ToList();

            return View();
        }

        public ActionResult Bestsellers()
        {

            var list = db.Order_Products
                         .Join(db.Product,
                             c => c.product_id,
                             p => p.product_id,
                             (c, p) => new {
                                 product_name = p.product_name
                             }).GroupBy(info => info.product_name)
                         .Select(group => new {
                             Product_Name = group.Key,
                             Count = group.Count()
                         }).OrderByDescending(x => x.Count).Take(25).ToList();



            List<int> repartitions = new List<int>();
            var product_List = list.Select(x => x.Product_Name);
            ViewBag.product = product_List;
            ViewBag.rep = list.Select(x => x.Count);

            return View();
        }

        public ActionResult NewOrderVsReorder()
        {

            var list = db.Order_Products.OrderBy(p => p.reordered).Select(p => new {
                reordered = p.reordered == 1 ? "Re-Order" : "New Order"
            }).ToList();
            List<int> repartitions = new List<int>();
            var hod_List = list.Select(x => x.reordered).Distinct();

            foreach (var item in hod_List)
            {
                repartitions.Add(list.Count(x => x.reordered == item));
            }

            var rep = repartitions;
            ViewBag.hod = hod_List;
            ViewBag.rep = repartitions.ToList();

            return View();

        }

        public ActionResult MostOftenReordered()
        {
            var list = db.Order_Products
                         .Join(db.Product,
                             c => c.product_id,
                             p => p.product_id,
                             (c, p) => new {
                                 product_name = p.product_name,
                                 reordered = c.reordered
                             }).GroupBy(y => y.product_name)
                         .Select(group => new {
                             Product_Name = group.Key,
                             Average = group.Average(r => r.reordered),
                             Count = group.Count()
                         }).OrderByDescending(x => x.Average).ToList();

            list.RemoveAll(p => p.Count <= 40);
            var list_Top25 = list.Take(25);
            ViewBag.product = list_Top25.Select(x => x.Product_Name);
            ViewBag.rep = list_Top25.Select(x => x.Average);

            return View();

        }

        public ActionResult OrganicVsNonOrganic()
        {
            var query = (from pt in db.Product
                        select new
                        {
                            product = pt.product_name.Contains("Organic")? "Organic": "Non Organic"
                            
                        }).ToList();

            var product = query.Select(x => x.product).Distinct();
            List<int> repartitions = new List<int>();

            foreach (var item in product)
            {
                repartitions.Add(query.Count(x => x.product == item));
            }

            ViewBag.product = product;
            ViewBag.rep = repartitions.ToList();

            return View();

        }

        public ActionResult ReOrganicVsNonOrganic()
        {
            var list = db.Order_Products
                         .Join(db.Product,
                             c => c.product_id,
                             p => p.product_id,
                             (c, p) => new {
                                 product_name = p.product_name,
                                 reordered = c.reordered
                             }).GroupBy(y => y.product_name)
                         .Select(group => new {
                             Product_Name = group.Key.Contains("Organic") ? "Organic" : "Non Organic",
                             Avg = (group.Sum(r => r.reordered) != 0) ?  group.Count() / group.Sum(r => r.reordered)    : 0
                         }).ToList();

            var product = list.Select(x => x.Product_Name).Distinct();

            List<int> repartitions = new List<int>();

            foreach (var item in product)
            {
                repartitions.Add(list.Where(x => x.Product_Name == item).Sum(x => x.Avg));
            }

            ViewBag.product = product;
            ViewBag.rep = repartitions.ToList();

            return View();

        }

        public ActionResult ItemOnCartFirst()
        {
            var list = db.Order_Products
                         .Join(db.Product,
                             c => c.product_id,
                             p => p.product_id,
                             (c, p) => new {
                                 product_name = p.product_name,
                                 add_to_cart_order = c.add_to_cart_order
                             }).Where(a => a.add_to_cart_order == 1).
                             GroupBy(y => new { y.product_name})
                         .Select(group => new {
                             Product_Name = group.Key.product_name,
                             Add_To_Cart_Order = group.Sum(r => r.add_to_cart_order),
                         }).OrderByDescending(x => x.Add_To_Cart_Order).Take(25).ToList();

            ViewBag.product = list.Select(x => x.Product_Name);
            ViewBag.rep = list.Select(x => x.Add_To_Cart_Order);

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