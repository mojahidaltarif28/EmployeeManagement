# Employee Management System

This is a web-based **Employee Management System** built using **.NET** and **SQL Server**. The system allows users to manage employee records, including image handling (with compression), sorting, pagination, and search functionality.

## Features

- **Employee Management**: Add, edit, delete employee records.
- **Image Handling**: Upload and resize employee images with compression.
- **Sorting & Pagination**: Sort columns and paginate employee data for easy navigation.
- **Search Functionality**: Search employees by name or email (supports partial matches).
- **Confirmation on Delete**: Prompts user confirmation before deleting any record.

## Technologies Used

- **Backend**: .NET (Razor Pages), C#
- **Database**: SQL Server
- **Frontend**: HTML, CSS, JavaScript
- **Libraries**: 
  - Bootstrap (for styling and responsive design)
  - Font Awesome (for icons)
  - jQuery (for interactive features)
  - **System.Drawing** (for image processing and compression)

## Image Compression

The project uses the **System.Drawing** namespace to handle image uploads and compress them before saving to the database. The compression process involves adjusting the JPEG quality to reduce file size while maintaining reasonable image quality.

- **Libraries** used for image compression:
  - `System.Drawing.Image`
  - `System.Drawing.Imaging.ImageCodecInfo`
  - `System.Drawing.Imaging.EncoderParameters`

## Clone the repository:

   ```bash
   git clone https://github.com/mojahidaltarif28/EmployeeManagement.git
