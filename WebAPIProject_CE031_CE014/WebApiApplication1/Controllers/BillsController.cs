using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApiApplication1.Models;

namespace WebApiApplication1.Controllers
{
    public class BillsController : ApiController
    {
        string connetionString;
        SqlConnection cnn;

        public IEnumerable<Bill> Get()
        {
            connetionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ShopDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            cnn = new SqlConnection(connetionString);
            List<Bill> lst = new List<Bill>();
            cnn.Open();
            string command = "select * from Bills";
            SqlCommand cmd = new SqlCommand(command, cnn);
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Bill b = new Bill();
                b.id = (int)rdr["Billid"];
                b.date= Convert.ToDateTime(rdr["Date"]);
                b.total = (int)rdr["Total"];
                b.Name = rdr["Name"].ToString();
               
                lst.Add(b);
            }
            cnn.Close();
            return lst;
        }

        public IEnumerable<History> Get(int id)
        {
            connetionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ShopDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            cnn = new SqlConnection(connetionString);
            List<History> lst = new List<History>();
            cnn.Open();
            string command = "select * from History where BillNo="+id;
            SqlCommand cmd = new SqlCommand(command, cnn);
            SqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                History h = new History();
                h.BillNo =Convert.ToInt32(rdr["BillNo"]);
                h.productid = Convert.ToInt32(rdr["Productid"]);
                h.Price = Convert.ToInt32(rdr["Price"]);
                h.Quantity = Convert.ToInt32(rdr["Quantity"]);
                h.Total = Convert.ToInt32(rdr["Total"]);
                lst.Add(h);
            }
            cnn.Close();
            return lst;
        }

        public HttpResponseMessage Post([FromBody] Bill bill)
        {
            connetionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ShopDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            try
            {
                var sql = "INSERT INTO Bills(Date,Total,Name) output INSERTED.Billid VALUES(@date,@total,@name1)";
                using (var cmd = new SqlCommand(sql, cnn))
                {
                    cmd.Parameters.AddWithValue("@date", bill.date);
                    cmd.Parameters.AddWithValue("@total", bill.total);
                    cmd.Parameters.AddWithValue("@name1", bill.Name);
                    int modified = (int)cmd.ExecuteScalar();

                    cnn.Close();
                    return Request.CreateResponse(HttpStatusCode.OK, modified);
                }
            }
            catch (Exception e)
            {
                cnn.Close();
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, e);
            }
        }
            
    }
}
