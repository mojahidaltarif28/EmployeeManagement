using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;

namespace EmployeeManagement.Pages.Customers
{
    public class AddEmployee : PageModel
    {
        [BindProperty, Required(ErrorMessage = "The First Name is required")]
        public string customer_name_f { get; set; } = "";
        [BindProperty, Required(ErrorMessage = "The Last Name is required")]
        public string customer_name_l { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "The Email is required")]
        public string email { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "The Mobile is required")]
        public string mobile { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "The Date of Birth is required")]
        public string dob { get; set; } = "";

        [BindProperty, Required(ErrorMessage = "The Image is required")]
        public IFormFile ImageFile { get; set; }

        public string ErrorMessage { get; set; } = "";

        public void OnGet() { }

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
                    string sql = "INSERT INTO Customers (customer_name,email,mobile,dob,image) VALUES (@customer_name,@email,@mobile,@dob,@image)";
                    using (SqlCommand command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@customer_name", customer_name_f + " " + customer_name_l);
                        command.Parameters.AddWithValue("@email", email);
                        command.Parameters.AddWithValue("@mobile", mobile);
                        command.Parameters.AddWithValue("@dob", dob);
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
