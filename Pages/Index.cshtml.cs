using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EmployeeManagement.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }
        public List<CustomerInfo> CustomerList { get; set; } = new List<CustomerInfo>();

        public string SortColumn { get; set; }
        public string SortOrder { get; set; }

        public int PageSize { get; set; } = 8;
        public int CurrentPage { get; set; } = 1;
        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);



        public void OnGet(string sortColumn = "customer_name", string sortOrder = "asc", int currentPage = 1, int pageSize = 8, string searchName = "", string searchDob = "", string searchEmail = "", string searchMobile = "")
        {
            SortColumn = sortColumn;
            SortOrder = sortOrder;
            CurrentPage = currentPage;
            PageSize = pageSize;

            try
            {
                string connectionString = "Server=localhost;Database=employeeDB;Trusted_Connection=True;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                   
                    connection.Open();

                    string filterQuery = "WHERE 1=1";
                    if (!string.IsNullOrEmpty(searchName))
                        filterQuery += " AND customer_name LIKE @searchName";
                    if (!string.IsNullOrEmpty(searchDob))
                        filterQuery += " AND dob = @searchDob";
                    if (!string.IsNullOrEmpty(searchEmail))
                        filterQuery += " AND email LIKE @searchEmail";
                    if (!string.IsNullOrEmpty(searchMobile))
                        filterQuery += " AND mobile LIKE @searchMobile";

                    string countSql = $"SELECT COUNT(*) FROM Customers {filterQuery}";
                    using (SqlCommand countCommand = new SqlCommand(countSql, connection))
                    {
                        if (!string.IsNullOrEmpty(searchName))
                            countCommand.Parameters.AddWithValue("@searchName", $"%{searchName}%");
                        if (!string.IsNullOrEmpty(searchDob))
                            countCommand.Parameters.AddWithValue("@searchDob", searchDob);
                        if (!string.IsNullOrEmpty(searchEmail))
                            countCommand.Parameters.AddWithValue("@searchEmail", $"%{searchEmail}%");
                        if (!string.IsNullOrEmpty(searchMobile))
                            countCommand.Parameters.AddWithValue("@searchMobile", $"%{searchMobile}%");

                        TotalRecords = (int)countCommand.ExecuteScalar();
                    }

                    string sql = $@"SELECT * FROM Customers {filterQuery} ORDER BY {SortColumn} {SortOrder.ToUpper()} OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@Offset", (CurrentPage - 1) * PageSize);
                        command.Parameters.AddWithValue("@PageSize", PageSize);
                        if (!string.IsNullOrEmpty(searchName))
                            command.Parameters.AddWithValue("@searchName", $"%{searchName}%");
                        if (!string.IsNullOrEmpty(searchDob))
                            command.Parameters.AddWithValue("@searchDob", searchDob);
                        if (!string.IsNullOrEmpty(searchEmail))
                            command.Parameters.AddWithValue("@searchEmail", $"%{searchEmail}%");
                        if (!string.IsNullOrEmpty(searchMobile))
                            command.Parameters.AddWithValue("@searchMobile", $"%{searchMobile}%");

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            CustomerList.Clear();
                            while (reader.Read())
                            {
                                CustomerInfo customerInfo = new CustomerInfo
                                {
                                    Id = reader.GetInt32(0),
                                    customer_name = reader.GetString(1),
                                    email = reader.GetString(2),
                                    mobile = reader.GetString(3),
                                    dob = reader.GetDateTime(4).ToString("MM/dd/yyyy"),
                                    image = reader.IsDBNull(5) ? null : (byte[])reader["image"]
                                };
                                CustomerList.Add(customerInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred");
            }
        }

    }

    public class CustomerInfo
    {
        public int Id { get; set; }
        public string customer_name { get; set; } = "";
        public string email { get; set; } = "";
        public string mobile { get; set; } = "";
        public string dob { get; set; } = "";
        public byte[] image { get; set; }
    }
}
