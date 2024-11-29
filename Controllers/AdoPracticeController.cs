using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Dynamic;

namespace Ado_practice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdoPracticeController : ControllerBase
    {
        [HttpGet("getall1")]
        public IActionResult getBanking()
        {
            string ConnectionString = "data source=DESKTOP-2B009SD\\SQLEXPRESS; database=arun; integrated security=SSPI;TrustServerCertificate=true;";
            List<dynamic> bankingData = new List<dynamic>();

            using (SqlConnection connection = new SqlConnection (ConnectionString))
            {
                connection.Open ();

                SqlCommand cmd = new SqlCommand("select * from banking",connection);
                SqlDataReader reader = cmd.ExecuteReader ();
                while (reader.Read())
                {
                    var row = new ExpandoObject() as IDictionary<string, Object>;
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row.Add(reader.GetName(i), reader.GetValue(i));
                    }
                    bankingData.Add(row);
                }
                return Ok (bankingData); 
            }

        }

        [HttpPost("addbanking")]
        public IActionResult postBanking([FromBody] int val, int Id) 
        {
          string ConnectionString = "data source=DESKTOP-2B009SD\\SQLEXPRESS; database=arun; integrated security=SSPI;TrustServerCertificate=true;";
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open ();
                SqlCommand cmd = new SqlCommand("insert into banking(Id,Balance) values(@id,@balance)", connection);
                cmd.Parameters.AddWithValue("@id",Id);
                cmd.Parameters.AddWithValue("@balance", val);
                int rowsAffected= cmd.ExecuteNonQuery();
                return rowsAffected > 0 ? Ok("Record added successfully.") : BadRequest("Failed to add record.");

            }

        }

        [HttpGet("datatable")]
       public IActionResult GetAllDataTable()
        {
          string ConnectionString = "data source=DESKTOP-2B009SD\\SQLEXPRESS; database=arun; integrated security=SSPI;TrustServerCertificate=true;";
            using (SqlConnection con=new SqlConnection(ConnectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter("select * from banking",con);
                DataTable dt = new DataTable();
                adapter.Fill(dt);
                var result = new List<Dictionary<string, object>>();
               foreach(DataRow row in dt.Rows)
                {
                    var dict = new Dictionary<string, object>();
                    foreach(DataColumn column in dt.Columns)
                    {
                        dict[column.ColumnName] = row[column];
                    }
                    result.Add(dict);
                }
                return Ok(result);
            }
        }
        [HttpPut("updateBalance")]
        public IActionResult UpdateBAlance(int CustomerId, int NewBalance)
        {
            string ConnectionString = "data source=DESKTOP-2B009SD\\SQLEXPRESS; database=arun; integrated security=SSPI;TrustServerCertificate=true;";
            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString)) 
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("UpdateBalance", con)) 
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
                        cmd.Parameters.AddWithValue("@NewBalance", NewBalance);
                        cmd.ExecuteNonQuery();
                        return Ok("balance updated");
                    }
                }

            }
            catch (Exception ex) 
            {   
                throw new Exception(ex.Message);
            }


        }


        [HttpPut("storedprocedurewith_output")]
        public IActionResult updateBalanceWithOutput(int CustomerId, int NewBalance)
        {
            string ConnectionString = "data source=DESKTOP-2B009SD\\SQLEXPRESS; database=arun; integrated security=SSPI;TrustServerCertificate=true;";

            try
            {
                using (SqlConnection con = new SqlConnection(ConnectionString))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("UpdateBalanceWithOutput", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@CustomerId", CustomerId);
                        cmd.Parameters.AddWithValue("@NewBalance", NewBalance);
                        SqlParameter outputMessage = new SqlParameter("@Message", SqlDbType.NVarChar, 255)
                        {
                            Direction = ParameterDirection.Output
                        };
                        cmd.Parameters.Add(outputMessage);
                        cmd.ExecuteNonQuery();
                        string message = outputMessage.Value.ToString();
                        return Ok(new { Message = message });
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }




    }
}
