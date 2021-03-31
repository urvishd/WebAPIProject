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
    public class ValuesController : ApiController
    {
        string connetionString;
        SqlConnection cnn;

        // GET api/values
        public IEnumerable<Product> Get( )
        {
            connetionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ShopDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            cnn = new SqlConnection(connetionString);
            List<Product> lst = new List<Product>();
            cnn.Open();
            string command = "select * from Products";
            SqlCommand cmd = new SqlCommand(command, cnn);
            SqlDataReader rdr = cmd.ExecuteReader();
           
            while (rdr.Read())
            {
                Product pd = new Product();
                pd.ProductId = (int)rdr["Id"];
                pd.ProductName = rdr["Name"].ToString();
                pd.ProductPrice = (int)rdr["Price"];
                pd.Quantity = (int)rdr["Quantity"];
                pd.Description = rdr["Description"].ToString();
                lst.Add(pd);
            }
            cnn.Close();
            return lst;
        }



        // GET api/values/5
        public HttpResponseMessage Get(int id)
        {
            connetionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ShopDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            cnn = new SqlConnection(connetionString);
            List<Product> lst = new List<Product>();
            cnn.Open();
            string command = "select * from Products where Id="+id;
            SqlCommand cmd = new SqlCommand(command, cnn);
            SqlDataReader rdr = cmd.ExecuteReader();
            if(!rdr.HasRows)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Product with ID:" + id + " not found");
            }
            while (rdr.Read())
            {
                Product pd = new Product();
                pd.ProductId = (int)rdr["Id"];
                pd.ProductName = rdr["Name"].ToString();
                pd.ProductPrice = (int)rdr["Price"];
                pd.Quantity = (int)rdr["Quantity"];
                pd.Description = rdr["Description"].ToString();
                lst.Add(pd);
            }
            cnn.Close();
            return Request.CreateResponse(HttpStatusCode.OK, lst);
        }


        // POST api/values
        public HttpResponseMessage Post([FromBody] Product product)
        {
            connetionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ShopDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            if (product.ProductId==0)
            {
                try
                {
                    var sql = "INSERT INTO Products(Name,Price,Quantity,Description) VALUES(@pname,@pprice,@qun,@des)";
                    using (var cmd = new SqlCommand(sql, cnn))
                    {
                        cmd.Parameters.AddWithValue("@pname", product.ProductName);
                        cmd.Parameters.AddWithValue("@pprice", product.ProductPrice);
                        cmd.Parameters.AddWithValue("@qun", product.Quantity);
                        cmd.Parameters.AddWithValue("@des", product.Description);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    cnn.Close();
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Error ocuured updating the Product!");
                }
            }
            else
            {
                try
                {
                    var sql = "INSERT INTO Products(Id,Name,Price,Quantity,Description) VALUES(@id1,@pname,@pprice,@qun,@des)";
                    using (var cmd = new SqlCommand(sql, cnn))
                    {
                        cmd.Parameters.AddWithValue("@id1", product.ProductId);
                        cmd.Parameters.AddWithValue("@pname", product.ProductName);
                        cmd.Parameters.AddWithValue("@pprice", product.ProductPrice);
                        cmd.Parameters.AddWithValue("@qun", product.Quantity);
                        cmd.Parameters.AddWithValue("@des", product.Description);
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    cnn.Close();
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound,"ProductID will be generated by system, you can't manually provide it!");
                }
            }



            cnn.Close();
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        // PUT api/values/5
        public HttpResponseMessage Put( [FromBody] Product product)
        {
            connetionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ShopDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            var sql = "UPDATE Products set Name=@pname,Price=@pprice,Quantity=@qun,Description=@des where Id=@pid";
            using (var cmd = new SqlCommand(sql, cnn))
            {
                cmd.Parameters.AddWithValue("@pid", product.ProductId);
                cmd.Parameters.AddWithValue("@pname", product.ProductName);
                cmd.Parameters.AddWithValue("@pprice", product.ProductPrice);
                cmd.Parameters.AddWithValue("@qun", product.Quantity);
                cmd.Parameters.AddWithValue("@des", product.Description);
                cmd.ExecuteNonQuery();
            }
            cnn.Close();
            return Request.CreateResponse(HttpStatusCode.OK,"Product updated successfully!!");
        }

        // DELETE api/values/5
        public HttpResponseMessage Delete(int id)
        {
            connetionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=ShopDatabase;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
            cnn = new SqlConnection(connetionString);
            cnn.Open();
            try
            {
                var sql = "DELETE from Products where Id=" + id;
                using (var cmd = new SqlCommand(sql, cnn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            catch(Exception e)
            {
                cnn.Close();
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, e);
                
            }
           
            cnn.Close();
            return Request.CreateResponse(HttpStatusCode.OK, "Product deleted successfully!!");
        }
    }
}
