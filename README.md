# Xây dựng Website Bán Giày — ASP.NET Core MVC

> Đồ án môn học Chuyên đề ASP.NET

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-MVC-512BD4)
![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB-CC2927?logo=microsoftsqlserver)

## Tổng quan

Hệ thống website thương mại điện tử bán giày được xây dựng trên nền tảng **ASP.NET Core 10 MVC**, sử dụng **Entity Framework Core** kết nối **SQL Server**. Hệ thống gồm hai phần chính:

- **Storefront** — Cửa hàng trực tuyến cho khách hàng (template Footwear / Bootstrap 4).
- **Admin Panel** — Trang quản trị cho người quản lý (template Sneat / Bootstrap 5).

URL được thiết kế thân thiện SEO với tiếng Việt không dấu (ví dụ: `/san-pham/giay-nike-air-max`).

## Các chức năng chính

### Quản trị (Admin)
1. Đăng nhập / Đăng xuất trang quản trị
2. Dashboard thống kê doanh thu, đơn hàng, biểu đồ 30 ngày, top sản phẩm bán chạy, cảnh báo tồn kho thấp
3. Quản lý danh mục, sản phẩm, biến thể sản phẩm (size / color / SKU / tồn kho)
4. Quản lý đơn hàng — hỗ trợ Export CSV / JSON
5. Quản lý bài viết (TinyMCE), liên hệ, slider, khách hàng, vai trò
6. Cấu hình thông tin cửa hàng, mạng xã hội, SEO

### Cửa hàng (Storefront)
1. Trang chủ với slider, danh mục, sản phẩm nổi bật
2. Danh sách & chi tiết sản phẩm theo biến thể
3. Tìm kiếm sản phẩm theo từ khóa
4. Giỏ hàng
5. Thanh toán Guest Checkout (COD — Cash on Delivery)
6. Tra cứu đơn hàng theo mã
7. Bài viết / tin tức, form liên hệ

## Công nghệ sử dụng

| Loại | Công nghệ |
|---|---|
| Backend | ASP.NET Core 10 MVC, C# |
| ORM | Entity Framework Core 10 (Code-First + Fluent API) |
| Database | SQL Server LocalDB / SQL Server 2019+ |
| Authentication | Cookie Authentication, Claims-based |
| Frontend | Bootstrap 4 (storefront), Bootstrap 5 (admin), jQuery |
| Editor | TinyMCE 7 |
| Charts | ApexCharts |

### NuGet Packages chính

| Package | Version | Mục đích |
|---|:---:|---|
| `Microsoft.EntityFrameworkCore.SqlServer` | 10.0.2 | EF Core SQL Server Provider |
| `Microsoft.EntityFrameworkCore.Tools` | 10.0.2 | EF Migrations CLI Tools |
| `Microsoft.VisualStudio.Web.CodeGeneration.Design` | 10.0.2 | MVC Scaffolding |

## Yêu cầu hệ thống

- [.NET SDK 10.0+](https://dotnet.microsoft.com/download)
- SQL Server LocalDB (đi kèm Visual Studio) hoặc SQL Server 2019+
- Visual Studio 2022 (17.12+) / VS 2026 Insiders / VS Code + C# Dev Kit
- (Khuyến nghị) [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/sql/ssms/) để import database

## Hướng dẫn cài đặt

### Bước 1 — Tải source code

```bash
git clone https://github.com/hiepnm94-tvu/ASPNET-DK24TTC5-nguyenminhhiep-shoe.git
cd ASPNET-DK24TTC5-nguyenminhhiep-shoe/src/quanlybangiay
```

Hoặc tải file ZIP, giải nén và `cd` vào thư mục `src/quanlybangiay`.

### Bước 2 — Import database

File SQL mẫu nằm tại `src/quanlybangiay/database/quanlybangiay.sql`.

**Cách 1 — Dùng SSMS (khuyến nghị):**

1. Mở SSMS, kết nối đến server `(localdb)\mssqllocaldb`.
2. Mở file `quanlybangiay.sql` (File → Open → File…).
3. Nhấn **Execute** (F5).

**Cách 2 — Dùng `sqlcmd` (CLI):**

```bash
sqlcmd -S "(localdb)\mssqllocaldb" -i src/quanlybangiay/database/quanlybangiay.sql
```

### Bước 3 — Cập nhật connection string

Mở `src/quanlybangiay/appsettings.json` và đổi `DefaultConnection` cho phù hợp với máy của bạn:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=quanlybangiay;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

Nếu dùng SQL Server full (không phải LocalDB), ví dụ:

```json
"DefaultConnection": "Server=localhost;Database=quanlybangiay;User Id=sa;Password=YourPassword;TrustServerCertificate=True;MultipleActiveResultSets=true"
```

### Bước 4 — Cài dev certificate cho HTTPS (lần đầu trên máy mới)

```bash
dotnet dev-certs https --trust
```

### Bước 5 — Restore, build và chạy ứng dụng

```bash
dotnet restore
dotnet build
dotnet run
```

Ứng dụng sẽ chạy mặc định tại:

- `https://localhost:5001`
- `http://localhost:5000`

(Cổng cụ thể sẽ hiển thị trong console khi khởi động.)

> 💡 Nếu dùng Visual Studio: mở file `quanlybangiay.slnx` và nhấn **F5**.

## Tài khoản quản trị mẫu

Truy cập: `https://localhost:{port}/Account/Login`

| Trường | Giá trị |
|---|---|
| Email | `hiepnm071194@tvu-onschool.edu.vn` |
| Password | `Hiep@123` |

> ⚠️ **Lưu ý bảo mật:** Đây là tài khoản mẫu chỉ dùng cho môi trường phát triển. Vui lòng đổi mật khẩu ngay sau khi đăng nhập lần đầu và không sử dụng trên môi trường production.

## Cấu trúc thư mục

```
src/quanlybangiay/
├── Program.cs                  # Entry point — cấu hình services & middleware
├── appsettings.json            # Cấu hình ứng dụng & connection string
├── quanlybangiay.csproj        # Project file
├── quanlybangiay.slnx          # Solution file
├── config/
│   └── DatabaseConfig.cs       # Extension đăng ký DbContext
├── Data/
│   └── ApplicationDbContext.cs # EF Core DbContext (12 DbSets)
├── Models/                     # Entity Models + ViewModels
├── Helpers/                    # FileUploadHelper, SessionExtensions
├── Controllers/                # Storefront Controllers (8)
├── Areas/Admin/                # Admin Area (Controllers + Views)
├── Views/                      # Razor Views storefront + Shared Layouts
├── wwwroot/                    # Static files (CSS, JS, images, uploads)
└── database/
    └── quanlybangiay.sql       # Script khởi tạo database
```

## Một số lỗi khi cài đặt

| Lỗi | Cách khắc phục |
|---|---|
| `Cannot open database "quanlybangiay"` | Chưa import file `.sql` — quay lại Bước 2 |
| `Login failed for user` | Kiểm tra lại connection string trong `appsettings.json` |
| Trình duyệt cảnh báo HTTPS không an toàn | Chạy lại `dotnet dev-certs https --trust` |
| `dotnet: command not found` | Cài đặt [.NET SDK 10.0](https://dotnet.microsoft.com/download) |
| Port 5000/5001 bị chiếm | Chỉnh `applicationUrl` trong `Properties/launchSettings.json` |
| Lỗi upload ảnh | Kiểm tra quyền ghi vào `wwwroot/uploads/` và kích thước file ≤ 5MB |

## Templates sử dụng

- **Admin:** [Sneat — Bootstrap 5 Admin Template](https://themewagon.com/themes/free-responsive-bootstrap-5-html5-admin-template-sneat/)
- **Storefront:** [Footwear — Bootstrap 4 eCommerce Template](https://themewagon.com/themes/free-bootstrap-4-html5-ecommerce-website-template-footwear/)

## Tác giả

- **Nguyễn Minh Hiệp** — Lớp DK24TTC5
- Email: minhhiep.q@gmail.com
- Số điện thoại: 0376542578
