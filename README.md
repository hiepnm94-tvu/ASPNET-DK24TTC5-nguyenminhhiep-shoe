# Đề tài: Xây dựng website bán giày

## Tổng quan

## Các chức năng chính của website
### Quản trị
<ol>
  <li>Đăng nhập, Đăng xuất trang quản trị</li>
  <li>Quản lý chuyên mục (Thêm, Sửa, Xóa)</li>
  <li>Quản lý sản phẩm (Thêm, Sửa, Xóa)</li>
  <li>Quản lý bài viết (Thêm, Sửa, Xóa)</li>
  <li>Quản lý liên hệ (Sửa, Xóa)</li>
  <li>Quản lý khách hàng (Thêm, Sửa, Xóa)</li>
  <li>Quản lý đơn hàng (Sửa, Xóa)</li>
</ol>

### Cửa hàng
<ol>
  <li>Danh mục sản phẩm</li>
  <li>Tìm kiếm sản phẩm</li>
  <li>Chi tiết sản phẩm</li>
  <li>Giỏ hàng (cart)</li>
  <li>Thanh toán (checkout)</li>
  <li>Theo dõi đơn hàng</li>
  <li>Liên hệ (contact)</li>
</ol>

## Hướng dẫn cài đặt

### Yêu cầu hệ thống:
- .NET SDK 10.0 trở lên
- SQL Server LocalDB hoặc SQL Server
- Visual Studio 2022/2025 hoặc VS Code

### Các bước thực hiện
1. Download và giải nén source code
2. Di chuyển vào thư mục dự án: cd src/quanlybangiay
3. Restore NuGet packages: `dotnet restore`
4. Cập nhật connection string trong `appsettings.json`

### Danh sách NuGet Packages
| Package   |      Version      |  Mục đích |
|----------|:-------------:|------:|
| `Microsoft.EntityFrameworkCore.SqlServer` |  10.0.2 | EF Core SQL Server Provider |
| `Microsoft.EntityFrameworkCore.Tools` |    10.0.2   |   EF Migrations CLI Tools |
| `Microsoft.VisualStudio.Web.CodeGeneration.Design` | 10.0.2 |    MVC Scaffolding |

### Import database
Đảm bảo rằng bạn đã cài đặt SQL Server thành công.
Data mẫu của project ở thư mục src/quanlybangiay/database/quanlybangiay.sql. Vui lòng import trước khi build code
### Tài khoản quản trị
Đường dẫn vào kênh quản trị:

https://localhost:{port}/Account/Login

Email: hiepnm071194@tvu-onschool.edu.vn

Password: Hiep@123

## Template admin dashboard
- https://themewagon.com/themes/free-responsive-bootstrap-5-html5-admin-template-sneat/
## Template frontend
- https://themewagon.com/themes/free-bootstrap-4-html5-ecommerce-website-template-footwear/

## Thông tin liên lạc
- Email: minhhiep.q@gmail.com
- Số điện thoại: 0376542578
