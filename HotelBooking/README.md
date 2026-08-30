# 🏨 HỆ THỐNG ĐẶT PHÒNG KHÁCH SẠN CAO CẤP (LUXURY HOTEL BOOKING)
> **Đồ Án Tốt Nghiệp / Demo Website Thuyết Trình Chuyên Nghiệp**  
> Xây dựng trên nền tảng **ASP.NET Core 8.0 MVC**, **Entity Framework Core**, **SQL Server LocalDB** và **Bootstrap 5.3**.

---

## 📌 I. TỔNG QUAN DỰ ÁN & TÍNH NĂNG NỔI BẬT

Website được thiết kế theo phong cách hiện đại (Modern Travel UI) với đầy đủ 2 phân hệ độc lập: **Phía Khách Hàng (Customer Frontend)** và **Phân Hệ Quản Trị (Admin Portal)**.

### ✨ Phía Khách Hàng (Customer Frontend):
1. **Trang Chủ (`/`)**:
   - Hero Banner sang trọng + Widget tìm kiếm khách sạn thông minh (Điểm đến, Ngày nhận, Ngày trả, Số khách).
   - Danh sách các điểm đến du lịch hot nhất Việt Nam (Đà Nẵng, Phú Quốc, Hà Nội, TP.HCM, Nha Trang, Sapa).
   - Lưới khách sạn 5 sao nổi bật, huy hiệu sao, điểm đánh giá, tiện ích và giá tốt nhất.
   - Banner ưu đãi mùa hè (Voucher giảm 15%), 4 lý do chọn chúng tôi và đánh giá từ du khách.
2. **Trang Danh Sách Khách Sạn & Bộ Lọc Đa Tiêu Chí (`/Hotel`)**:
   - Bộ lọc theo Thành phố, Khoảng giá (Min - Max), Hạng sao (5 sao, 4 sao, 3 sao), Điểm đánh giá (9.0+, 8.0+).
   - Sắp xếp linh hoạt: Nổi bật nhất, Giá tăng dần, Giá giảm dần, Đánh giá cao nhất.
3. **Trang Chi Tiết Khách Sạn (`/Hotel/Detail/{id}`)**:
   - Gallery hình ảnh, tổng quan, danh sách tiện ích trực quan với biểu tượng.
   - Nút **Lưu Yêu Thích (AJAX Wishlist)**.
   - Bảng danh sách các loại phòng còn trống kèm thông số (Sức chứa, Giường, Diện tích, Tiện nghi phòng).
   - Xem đánh giá thực tế và **Form gửi đánh giá/chấm sao trực tiếp**.
4. **Trang Chi Tiết Hạng Phòng (`/Room/Detail/{id}`)**:
   - Xem chi tiết thông số kỹ thuật phòng.
   - **Bảng Tính Giá Real-Time (Live Price Calculator)**: Tự động tính tiền phòng, Thuế VAT 8%, Phí dịch vụ 5% và Tổng tiền thanh toán ngay khi đổi ngày hoặc số lượng phòng.
5. **Quy Trình Đặt Phòng & Thanh Toán Demo (`/Booking/Checkout` & `/Booking/Payment/{id}`)**:
   - Điền thông tin người đặt phòng (Tự động điền nếu đã đăng nhập).
   - Bảng tóm tắt chi phí minh bạch.
   - **Mô phỏng 4 Cổng Thanh Toán Demo**: Cổng VNPAY-QR, Ví MoMo, Chuyển khoản VietQR, Thanh toán tại quầy.
6. **Xác Nhận Đặt Phòng & In Hóa Đơn (`/Booking/Success/{id}`)**:
   - Tự động sinh mã **Booking Code** độc nhất dạng `HB-20260819-XXXX`.
   - Nút **In Phiếu Đặt Phòng (`window.print()`)** phục vụ trình diễn.
7. **Quản Lý Tài Khoản Khách Hàng (`/Account/Profile` & `/Booking/History`)**:
   - Xem & Cập nhật hồ sơ, Đổi mật khẩu.
   - Quản lý **Lịch sử đặt phòng (Booking History)** với Badge trạng thái (`Confirmed`, `Pending`, `CheckedIn`, `Completed`, `Cancelled`).
   - **Hủy đặt phòng trực tiếp** (Tự động hoàn lại số lượng phòng trống cho khách sạn).
   - Danh sách khách sạn yêu thích (`/Hotel/Favorites`).

---

### 👑 Phân Hệ Quản Trị Viên (Admin Portal - `/Admin`):
1. **Admin Dashboard (`/Admin/Dashboard`)**:
   - 4 Thẻ KPI thống kê: Doanh thu, Tổng đơn đặt, Tổng khách sạn/phòng, Người dùng & Đơn chờ duyệt.
   - **2 Biểu đồ trực quan (Chart.js)**: Biểu đồ cột Doanh thu 12 tháng và Biểu đồ tròn Tỷ lệ trạng thái đơn.
   - Bảng danh sách các đơn đặt phòng mới nhất.
2. **Quản Lý Khách Sạn (`/Admin/Hotels`)**: CRUD khách sạn, chọn tiện ích cung cấp, đổi trạng thái hoạt động.
3. **Quản Lý Hạng Phòng (`/Admin/Rooms`)**: CRUD phòng theo từng khách sạn, giá theo đêm, sức chứa, số lượng phòng trống.
4. **Quản Lý Đơn Đặt Phòng (`/Admin/Bookings`)**: Lọc theo trạng thái, chuyển đổi trạng thái nghiệp vụ nhanh (*Chờ duyệt -> Đã xác nhận -> Đang ở -> Đã trả phòng -> Hoàn tất / Hủy*).
5. **Quản Lý Người Dùng (`/Admin/Users`)**: Danh sách tài khoản, Khóa / Mở khóa (Lock/Unlock), Phân lại vai trò (Admin, Staff, Customer).
6. **Nhật Ký Giao Dịch (`/Admin/Payments`)**: Kiểm tra toàn bộ lịch sử thanh toán qua các cổng.
7. **Kiểm Duyệt Đánh Giá (`/Admin/Reviews`)**: Duyệt hiển thị, Ẩn hoặc Xóa đánh giá của khách hàng.

---

## 🔑 II. TÀI KHOẢN DEMO SẴN CÓ

Hệ thống đã tự động khởi tạo dữ liệu mẫu với các tài khoản:

| Vai Trò | Email Đăng Nhập | Mật Khẩu | Quyền Hạn |
| :--- | :--- | :--- | :--- |
| 👑 **Quản Trị Viên (Admin)** | `admin@hotelbooking.com` | `Admin@123` | Toàn quyền quản trị Dashboard, Khách sạn, Phòng, Bookings, Users, Payments, Reviews |
| 👔 **Nhân Viên (Staff)** | `staff@hotelbooking.com` | `Staff@123` | Quản lý Khách sạn, Phòng, Bookings, Reviews |
| 👤 **Khách Hàng (Customer)** | `customer@hotelbooking.com` | `Customer@123` | Đặt phòng, Thanh toán demo, Xem lịch sử, Đánh giá, Yêu thích |
| 👤 **Khách Hàng 2 (Demo)** | `demo.user@hotelbooking.com` | `Customer@123` | Khách hàng mẫu đã có sẵn đơn đặt phòng trong CSDL |

> [!TIP]
> Tại trang Đăng Nhập (`/Account/Login`), hệ thống đã tích hợp sẵn **2 nút 1-Click Auto Fill** cho tài khoản **Admin** và **Customer** giúp bạn thao tác nhanh chóng và mượt mà khi thuyết trình đồ án!

---

## 🚀 III. HƯỚNG DẪN CHẠY DỰ ÁN

1. Mở Terminal / PowerShell tại thư mục dự án `d:\HotelBooking`.
2. Chạy lệnh:
   ```bash
   dotnet run
   ```
3. Mở trình duyệt web và truy cập địa chỉ hiển thị trên terminal (thường là `https://localhost:7000` hoặc `http://localhost:5000`).

---

## 🎬 IV. KỊCH BẢN THUYẾT TRÌNH ĐỒ ÁN ĐỀ XUẤT (DEMO SCRIPT)

### 🔹 Phần 1: Giới thiệu Tổng quan & Trang Chủ Phía Khách Hàng (2-3 phút)
- **Hành động:** Truy cập Trang chủ `https://localhost:7000`.
- **Thuyết minh:** Giới thiệu giao diện hiện đại, chuẩn UI/UX du lịch; giới thiệu các điểm đến hot (Đà Nẵng, Phú Quốc, Sapa...), lưới khách sạn 5 sao nổi bật và các đánh giá từ khách hàng.
- **Thao tác:** Trên Widget tìm kiếm tại Hero Banner, nhập điểm đến *"Đà Nẵng"*, chọn ngày nhận phòng & trả phòng, chọn 2 khách -> Bấm **"Tìm Kiếm"**.

### 🔹 Phần 2: Trình diễn Tìm kiếm, Bộ Lọc & Chi Tiết Phòng (2-3 phút)
- **Hành động:** Trang danh sách khách sạn `/Hotel` hiện ra.
- **Thao tác:** Thử nghiệm bộ lọc bên trái (chọn lọc theo Hạng sao 5 sao, khoảng giá, sắp xếp theo *Giá thấp đến cao*) -> Bấm chọn khách sạn *InterContinental Danang*.
- **Thuyết minh:** Giới thiệu Gallery ảnh sang trọng, danh sách tiện ích, bấm thử nút **"Lưu Yêu Thích"** -> Cuộn xuống bảng danh sách các hạng phòng trống -> Bấm xem **"Chi Tiết Phòng"** (Hạng phòng Deluxe Ocean View).
- **Thuyết minh Điểm Nhấn:** Trình diễn **Bảng tính giá trực tuyến (Live Calculator)**: Thay đổi số đêm và số lượng phòng -> Cho giảng viên thấy tổng tiền, thuế VAT 8% và phí dịch vụ tự động nhảy số tức thì -> Bấm **"Tiến Hành Đặt Phòng"**.

### 🔹 Phần 3: Trình diễn Đặt Phòng & Thanh Toán Demo (2 phút)
- **Hành động:** Sang trang `/Booking/Checkout`.
- **Thao tác:** Điền thông tin khách hàng -> Chọn phương thức thanh toán **"VNPay (Mô Phỏng QR)"** -> Bấm **"Xác Nhận & Đặt Phòng"**.
- **Thao tác:** Sang màn hình `/Booking/Payment/{id}` -> Giới thiệu giao diện quét mã QR mô phỏng chuyên nghiệp -> Bấm **"Xác Nhận Đã Quét QR & Thanh Toán (Demo)"**.
- **Thuyết minh:** Chuyển sang trang `/Booking/Success/{id}` nhận ngay mã **Booking Code** -> Bấm thử nút **"In Phiếu Đặt Phòng"** để chứng minh tính thực tế của đồ án.

### 🔹 Phần 4: Trình diễn Phân Hệ Khách Hàng & Đánh Giá (1-2 phút)
- **Thao tác:** Đăng nhập tài khoản `customer@hotelbooking.com` (sử dụng nút 1-click login) -> Vào mục **"Lịch Sử Đặt Phòng"** xem đơn hàng vừa tạo với trạng thái *Đã Xác Nhận* -> Bấm xem chi tiết hoặc thử nghiệm tính năng *Hủy đơn phòng*.
- **Thao tác:** Vào khách sạn gửi 1 đánh giá 5 sao kèm nhận xét.

### 🔹 Phần 5: Trình diễn Phân Hệ Quản Trị Admin (3-4 phút)
- **Thao tác:** Đăng nhập tài khoản `admin@hotelbooking.com` (1-click login) -> Bấm nút **"Admin Portal"** trên thanh Header để vào `/Admin/Dashboard`.
- **Thuyết minh:**
  1. Giới thiệu các chỉ số KPI: Tổng doanh thu, Tổng đơn đặt, Tổng khách sạn/phòng.
  2. Giới thiệu **Biểu đồ cột Chart.js Doanh thu 12 tháng** và **Biểu đồ tròn Trạng thái đơn đặt**.
  3. Vào menu **"Quản Lý Đơn Đặt"** (`/Admin/Bookings`): Tìm thấy đơn đặt phòng vừa tạo ở Phần 3 -> Chuyển trạng thái sang *Đang Lưu Trú (CheckedIn)* hoặc *Đã Trả Phòng (CheckedOut)*.
  4. Vào menu **"Quản Lý Khách Sạn"** & **"Quản Lý Phòng"**: Trình diễn tính năng thêm mới/sửa phòng và cấu hình tiện ích.
  5. Vào menu **"Quản Lý Người Dùng"**: Trình diễn tính năng *Khóa/Mở khóa tài khoản* và *Phân quyền Role*.
  6. Vào menu **"Quản Lý Đánh Giá"**: Xem và kiểm duyệt đánh giá vừa gửi từ phía khách hàng.

---

> [!NOTE]
> Dự án được tối ưu 100% để chạy mượt mà trên máy tính của bạn với mã nguồn rõ ràng, kiến trúc phân tầng chuẩn MVC và dễ dàng giải thích trong phần hỏi đáp với hội đồng chấm thi!
