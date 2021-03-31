using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using WebApiApplication1.Models;

namespace WebApplication1_client.Controllers
{
    public class CheckoutController : Controller
    {
        // GET: Checkout
        static Dictionary<int, int> cart=new Dictionary<int, int>();
        static Dictionary<int, int> prices = new Dictionary<int, int>();
        static Dictionary<int, int> quantities = new Dictionary<int, int>();

        static int total = 0;
        public ActionResult Index(string pid1,string quantity,string pprice,string avl)
        {
            int id = Convert.ToInt32(pid1);
            int qnt = Convert.ToInt32(quantity);
            int price = Convert.ToInt32(pprice);
            int avlq = Convert.ToInt32(avl);

            if (cart.ContainsKey(id))
            {
                cart[id] = qnt;
                total += price * qnt;
            }
            else
            {
                prices[id] = price;
                quantities[id] = avlq;
                cart.Add(id, qnt);
                total += price * qnt;
            }
             
            return RedirectToAction("Index", "Home");

        }

        public ActionResult Viewcart()
        {
            /*string ans = "";
            foreach (var element in cart)
            {
                ans += element;
                ans += "    ";
            }
            ViewBag.ans = ans;*/
            ViewBag.total1 = total;
            return View(cart);
        }

        public ActionResult Removecart(string pid,string qnt)
        {
            int id1 = Convert.ToInt32(pid);
            int qnty = Convert.ToInt32(qnt);
            cart.Remove(id1);

            string connetionString;
            SqlConnection cnn;
            connetionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ShopDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            string command = "select Price from Products where Id=" + id1;
            SqlCommand cmd = new SqlCommand(command, cnn);
            SqlDataReader rdr = cmd.ExecuteReader();
            int ans = 0;
            while (rdr.Read())
            {
                ans = (int)rdr["Price"];
            }
            cnn.Close();

            total -= qnty * ans;
            
            return RedirectToAction("Viewcart",new {total1=total});
        }

        public ActionResult Viewbills(string pid)
        {
            IEnumerable<Bill> bills = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44372/api/");

                var responseTask = client.GetAsync("Bills");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<Bill>>();
                    readTask.Wait();

                    bills = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    ViewBag.msg = "Some error ocuured in Service";
                }
            }
            return View(bills);
        }

        public ActionResult Billdetail(string bid)
        {
            IEnumerable<History> history = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44372/api/");

                var responseTask = client.GetAsync("Bills/"+bid);
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<History>>();
                    readTask.Wait();

                    history = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    ViewBag.msg = "Some error ocuured in Service";
                }
            }
            return View(history);
        }

        public ActionResult Makebill(string custname)
        {

            if(cart.Count==0)
            {
                return RedirectToAction("Index", "Home", new { stat = "cartempty" });
            }
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44372/api/");
                Bill b = new Bill();

                b.date = DateTime.Now;
                b.total = total;
                b.Name = custname;
                //HTTP POST
                var postTask = client.PostAsJsonAsync<Bill>("Bills", b);

                postTask.Wait();
                
                var result = postTask.Result;
                var billid =result.Content.ReadAsStringAsync();

                string connetionString;
                SqlConnection cnn;
                connetionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ShopDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
                cnn = new SqlConnection(connetionString);
                cnn.Open();


                

                foreach (var itm1 in cart)
                {
                    int p = prices[itm1.Key];

                    var sql1 = "UPDATE Products set Quantity=@qun where Id=@pid";
                    using (var cmd = new SqlCommand(sql1, cnn))
                    {
                        cmd.Parameters.AddWithValue("@pid", itm1.Key);
                        cmd.Parameters.AddWithValue("@qun",quantities[itm1.Key]-itm1.Value );
                        cmd.ExecuteNonQuery();
                    }

                    var sql = "INSERT INTO History(BillNo,Productid,Price,Quantity,Total) VALUES(@billno,@pid,@pprice,@qun,@tot)";
                    using (var cmd = new SqlCommand(sql, cnn))
                    {
                        cmd.Parameters.AddWithValue("@billno", billid.Result);
                        cmd.Parameters.AddWithValue("@pid", itm1.Key);
                        cmd.Parameters.AddWithValue("@pprice", p);
                        cmd.Parameters.AddWithValue("@qun", itm1.Value);
                        cmd.Parameters.AddWithValue("@tot", p * itm1.Value);

                        cmd.ExecuteNonQuery();

                    }


                }

                if (result.IsSuccessStatusCode)
                {
                    cart.Clear();
                    total = 0;
                    ViewBag.addstatus = "Product added Successfully!!";
                    return RedirectToAction("Index","Home", new { stat = "billsuccess",billid1=billid.Result });
                }
            }
            return RedirectToAction("Viewcart");
        }



    }
}