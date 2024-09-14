using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Pages.Customers
{
    public class Delete : PageModel
    {

        [BindProperty]
        public string customer_name { get; set; } = "";
        public void OnGet(int id)
        {
            try
            {
                string connectionString = "Server=localhost;Database=employeeDB;Trusted_Connection=True;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "SELECT * FROM Customers WHERE id=@Id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                customer_name = reader.GetString(1);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot delete :" + ex.Message);
            }
        }
        public void OnPost(int id)
        {
            deleteCustomer(id);
            Response.Redirect("/Index");
        }
        private void deleteCustomer(int id)
        {
            try
            {
                string connectionString = "Server=localhost;Database=employeeDB;Trusted_Connection=True;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = "DELETE FROM Customers WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot delete :" + ex.Message);
            }
        }
    }
}