using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace EmployeeManagement.Pages.Customers
{
    public class Edit : PageModel
    {
        [BindProperty]
        public int Id { get; set; }

        [BindProperty, Required(ErrorMessage = "The Name is required")]
        public string customer_name { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "The Email is required")]
        public string email { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "The Mobile is required")]
        public string mobile { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "The Date of Birth is required")]
        public DateTime dob { get; set; }

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public string ErrorMessage { get; set; } = "";


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

                                Id = reader.GetInt32(0);
                                customer_name = reader.GetString(1);
                                email = reader.GetString(2);
                                mobile = reader.GetString(3);
                                dob = reader.GetDateTime(4);
                            }
                            else
                            {
                                Response.Redirect("/Index");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            byte[] compressedImage = null;
            if (ImageFile != null && ImageFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await ImageFile.CopyToAsync(memoryStream);
                    using (var img = System.Drawing.Image.FromStream(memoryStream))
                    {
                        compressedImage = CompressImage(img, 50);
                    }
                }
            }
            try
            {
                string connectionString = "Server=localhost;Database=employeeDB;Trusted_Connection=True;TrustServerCertificate=True";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    string sql = compressedImage != null ? "UPDATE Customers SET customer_name=@customer_name, email=@email, mobile=@mobile, dob=@dob, image=@image WHERE id=@id" :
                    "UPDATE Customers SET customer_name=@customer_name, email=@email, mobile=@mobile, dob=@dob WHERE id=@id";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {

                        command.Parameters.AddWithValue("@id", Id);
                        command.Parameters.AddWithValue("@customer_name", customer_name);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@mobile", mobile);
                        command.Parameters.AddWithValue("@dob", dob);
                        if (compressedImage != null)
                            command.Parameters.AddWithValue("@image", (object)compressedImage ?? DBNull.Value);
                        await command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return Page();
            }
            return RedirectToPage("/Index");

        }
        private byte[] CompressImage(Image img, long quality)
        {
            using (var ms = new MemoryStream())
            {
                var jpegEncoder = GetEncoder(ImageFormat.Jpeg);
                var encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
                img.Save(ms, jpegEncoder, encoderParams);
                return ms.ToArray();
            }
        }
        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }


    }
}