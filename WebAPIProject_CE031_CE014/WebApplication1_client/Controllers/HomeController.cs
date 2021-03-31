using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using WebApiApplication1.Models;

namespace WebApplication1_client.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            IEnumerable<Product> products = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44372/api/");

                var responseTask = client.GetAsync("Values");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Product>>();
                    readTask.Wait();

                    products = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    ViewBag.msg = "Some error ocuured in Service";
                }
            }
            return View(products);
        }


        public ActionResult GetProduct1()
        {
            return View();
        }

        public ActionResult AddProduct1()
        {
            return View();
        }

        public ActionResult DeleteProduct1()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetProduct(string pid)
        {
            if (ModelState.IsValid)
            {
                IEnumerable<Product> products = null;
                Product p = null;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44372/api/");

                    var responseTask = client.GetAsync("Values/" + pid);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<IList<Product>>();

                        readTask.Wait();
                        products = readTask.Result;
                        // ViewBag.msg = readTask.ToString();
                        foreach (Product pd in products)
                        {
                            p = pd;
                        }
                    }
                    else //web api sent error response 
                    {
                        //log response status here..

                        ViewBag.msg = "Invalid Product ID!!";
                        return View("GetProduct1");
                    }
                }

                return View(p);
            }
            //int id = Convert.ToInt32(pid);

            return View("GetProduct1");

        }

        public ActionResult AddProduct(string name,string price,string qnt,string des)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44372/api/");
                Product p = new Product();
                p.Description = des;
                p.ProductName = name;
                p.ProductPrice = (int)Convert.ToInt64(price);
                p.Quantity = Convert.ToInt32(qnt);
                //HTTP POST
                var postTask = client.PostAsJsonAsync<Product>("Values", p);
              
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    ViewBag.addstatus = "Product added Successfully!!";
                    return RedirectToAction("Index", new { stat = "addsuccess" });
                }
            }
            return View("AddProduct1");
        }

        
        public ActionResult UpdateProduct(string pid,string pname, string price, string qnt, string des)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44372/api/");
                Product p = new Product();
                p.ProductId =Convert.ToInt32(pid);
                p.Description = des;
                p.ProductName = pname;
                p.ProductPrice = (int)Convert.ToInt64(price);
                p.Quantity = Convert.ToInt32(qnt);
                //HTTP POST
                var postTask = client.PutAsJsonAsync<Product>("Values", p);

                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    ViewBag.addstatus = "Product updated Successfully!!";
                    return RedirectToAction("Index");
                }
            }
            return View();
        }

        [HttpPost]
        public ActionResult DeleteProduct(string pid)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44372/api/");

                //HTTP DELETE 
                var deleteTask = client.DeleteAsync("Values/" + pid);
                deleteTask.Wait();

                var result = deleteTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return RedirectToAction("Index", new { stat = "deletesuccess" });
                }
            }
            ViewBag.msg = "No product exist with Product ID:" + pid;
            return View("DeleteProduct1");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}