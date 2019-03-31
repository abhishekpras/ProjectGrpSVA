using LINQtoCSV;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProjectGrpSVA.DataAccess;
using ProjectGrpSVA.DataAccess.DL;
using ProjectGrpSVA.Models;
using RestSharp;
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

        public ActionResult About()
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
            ViewBag.explaination = "There is a clear effect of hour of day on order volume. Most orders are between 8.00-18.00";

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
            ViewBag.explaination = "There is a clear effect of day of the week. Most orders are on days 0 and 1. Unfortunately there is no info regarding which values represent which day, but one would assume that this is the weekend.";

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
            ViewBag.explaination = "People seem to order more often after exactly 1 Month.";


            return View();
        }

        public ActionResult ItemsDoPeopleBuy()
        {
            var list = (from s in db.Order_Products
                        group s by s.order_id into g
                        select new {
                            Order_Id = g.Key,
                            n_items = g.OrderByDescending(c => c.add_to_cart_order).FirstOrDefault()
                        }).ToList();

            ViewBag.rep = list.Select(x => x.Order_Id);
            ViewBag.hod = list.Select(x => x.n_items);

            ViewBag.explaination = "Let’s have a look how many items are in the orders. We can see that people most often order around 5 items.";

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
            ViewBag.explaination = "Let’s have a look which products are sold most often (top25). And the clear top 25 are winners";
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
            ViewBag.explaination = "More than 50% of the ordered items are reorders.";

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
            ViewBag.explaination = "Now here it becomes really interesting. These 25 products have the highest probability of being reordered.";

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

            ViewBag.explaination = "What is the percentage of orders that are organic vs. not organic?";

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

            ViewBag.explaination = "People more often reorder non-organic products vs organic products.";

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
            ViewBag.explaination = "People seem to be quite certain about their love for products and if they buy them, put them into their cart first in more than 60% of the time.";

            return View();

        }


        public ActionResult IncomePrediction(string text_age, string text_education,string text_martial_status, string text_relationship,string text_race,string text_sex)
        {
            string st_text_age = Request["text_age"];
            string st_text_education = Request["text_education"];
            string st_text_martial_status = Request["text_martial_status"];
            string st_text_relationship = Request["text_relationship"];
            string st_text_race = Request["text_race"];
            string st_text_sex = Request["text_sex"];
            string predict_greater = ">50K";

            String cAge = "Age: ";
            String cEducation = "  :Education: ";
            String cMartialstatus = "  :Martial Status: ";
            String cRelationship = "  :Relationship: ";
            String cRace = "  :Race: ";
            String cSex = "  :Sex: ";


            if (string.IsNullOrEmpty(st_text_age))
            {
                Debug.WriteLine("It is null");
                st_text_age = "40";
                st_text_education = "Doctorate";
                st_text_martial_status = "Married-AF-spouse";
                st_text_relationship = "Husband";
                st_text_race = "White";
                st_text_sex = "Male";
            }
            else
            {
                Debug.WriteLine("It is not null");
            }

            String user_input = String.Concat(cAge,st_text_age , cEducation,st_text_education, cMartialstatus, st_text_martial_status, cRelationship,st_text_relationship, cRace,st_text_race, cSex, st_text_sex);

            var client = new RestClient("https://ussouthcentral.services.azureml.net/workspaces/3393a5fc80a74e40bb902dafbaec5617/services/7510e8e34ba046539f08c275abe6076c/execute?api-version=2.0&format=swagger");
            var request = new RestRequest(Method.POST);
            request.AddHeader("P" +
                "ostman-Token", "2461affa-2b1a-4325-b550-7bc8e08d1b5e");
            request.AddHeader("cache-control" +
                "", "no-cache");
            request.AddHeader("Authorization", "Bearer ouBBRCNKH8O7PiLbzsGBnP05zvgdt7S2CFGe5k49DgZckVwcPjMe0x7Aek0DOFJE2cF1SjAWoAYqVppteoMhGQ==");
            request.AddHeader("Content-Type", "application/json");
            //request.AddParameter("undefined", "{\r\n        \"Inputs\": {\r\n                \"input1\":\r\n                [\r\n                    {\r\n                            'age': \"" + txtAge.Text + "\",   \r\n                            'education': \"" + drpEducation.Text + "\",   \r\n                            'marital-status': \"" + rbtMarriedStatus.SelectedItem.Text + "\",   \r\n                            'relationship': \"" + lboxRelation.Text + "\",   \r\n                            'race': \"" + ChkRace.Text + "\",   \r\n                            'sex': \"" + drpSex.Text + "\",   \r\n                    }\r\n                ],\r\n        },\r\n    \"GlobalParameters\":  {\r\n    }\r\n}\r\n", ParameterType.RequestBody);
            request.AddParameter("undefined", "{\r\n        \"Inputs\": {\r\n                \"input1\":\r\n                [\r\n                    {\r\n                            'age': \"" + st_text_age + "\",   \r\n                            'education': \"" + st_text_education + "\",   \r\n                            'marital-status': \"" + st_text_martial_status + "\",   \r\n                            'relationship': \"" + st_text_relationship + "\",   \r\n                            'race': \"" + st_text_race + "\",   \r\n                            'sex': \"" + st_text_sex + "\",   \r\n                    }\r\n                ],\r\n        },\r\n    \"GlobalParameters\":  {\r\n    }\r\n}\r\n", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            
            /*
            Debug.WriteLine("Age is:" + st_text_age);
            Debug.WriteLine("Education is:" + st_text_education);
            Debug.WriteLine("Martial Status is:" + st_text_martial_status);
            Debug.WriteLine("Relationship is:" + st_text_relationship);
            Debug.WriteLine("Race is:" + st_text_race);
            Debug.WriteLine("Sex is:" + st_text_sex);
            */
            
            var results = JObject.Parse(response.Content);
            string predict_value = results["Results"]["output1"].ToString();
            string predict_value1 = predict_value.Replace("[", "").Replace("]", "").Replace("{", "").Replace("}", "").Replace("\"", "");

            int first_index = predict_value1.IndexOf("Scored Labels:") + "Scored Labels:".Length;
            int last_index = predict_value1.LastIndexOf("Scored Probabilities:");
            string final_predict = predict_value1.Substring(first_index, last_index - first_index).Replace(",","");

            ViewBag.final_predict = final_predict;
           
            if (final_predict.Contains(predict_greater))
            {
                Debug.WriteLine("Income is greater than 50K");
                ViewBag.comment = ("His Predicted Income should be greater than 50K. Ask if he would like to apply for Credit Card");
                ViewBag.user_input = user_input;
            }
            else
            {
                Debug.WriteLine("Income is less than 50K");
                ViewBag.comment = ("His Predicted Income should be less than 50K. Ask if he would like to receive Coupons");
                ViewBag.user_input = user_input;
            }
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

        public ActionResult Contact()
        {
            return View();
        }

        public ActionResult MarketBasketAnalysis(string input_1, string input_2)
        {

            string first_input = Request["input_1"];
            string second_input = Request["input_2"];

             if (string.IsNullOrEmpty(first_input))
            {
                first_input = "sugar";
                second_input = "butter";
            }
            else
            {
                Debug.WriteLine("It is not null");
            }

            var client = new RestClient("https://ussouthcentral.services.azureml.net/workspaces/3393a5fc80a74e40bb902dafbaec5617/services/00337e43fd414d9b97d4db94653477ac/execute?api-version=2.0&format=swagger");
            var request = new RestRequest(Method.POST);
            request.AddHeader("Postman-Token", "185a2570-d2e2-4419-b835-057ed9cc7038");
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("Authorization", "Bearer BSBwa+SAlu7li0hStZYmNpZ4tpNNnSs6y8l+DSxBKJ/kGGL8wqrVx/1bkUof2n84UTomSnv6pHl5DpsGiEn0fg==");
            request.AddHeader("Content-Type", "application/json");
            //request.AddParameter("undefined", "{\r\n        \"Inputs\": {\r\n                \"input1\":\r\n                [\r\n                    {\r\n                            'Item1': \"sugar\",   \r\n                            'Item2': \"butter\",   \r\n                    }\r\n                ],\r\n        },\r\n    \"GlobalParameters\":  {\r\n    }\r\n}\r\n", ParameterType.RequestBody);
            request.AddParameter("undefined", "{\r\n        \"Inputs\": {\r\n                \"input1\":\r\n                [\r\n                    {\r\n                            'Item1': \"" + first_input + "\",   \r\n                            'Item2': \"" + second_input + "\",   \r\n                    }\r\n                ],\r\n        },\r\n    \"GlobalParameters\":  {\r\n    }\r\n}\r\n", ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);

            RootObject tmp = JsonConvert.DeserializeObject<RootObject>(response.Content);

            Results r = new Results();
            r = tmp.Results;

           return View(r.output1);
        }



    }
}