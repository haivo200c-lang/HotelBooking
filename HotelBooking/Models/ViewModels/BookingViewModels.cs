using System.ComponentModel.DataAnnotations;
using HotelBooking.Models.Entities;

namespace HotelBooking.Models.ViewModels
{
    public class CheckoutViewModel
    {
        // Hotel & Room Info
        public int HotelId { get; set; }
        public Hotel? Hotel { get; set; }

        public int RoomId { get; set; }
        public Room? Room { get; set; }

        // Booking Dates & Config
        [Required(ErrorMessage = "Vui lòng chọn ngày nhận phòng")]
        public DateTime CheckInDate { get; set; } = DateTime.Today.AddDays(1);

        [Required(ErrorMessage = "Vui lòng chọn ngày trả phòng")]
        public DateTime CheckOutDate { get; set; } = DateTime.Today.AddDays(2);

        [Range(1, 100, ErrorMessage = "Số lượng khách không hợp lệ")]
        public int TotalGuests { get; set; } = 2;

        [Range(1, 20, ErrorMessage = "Số lượng phòng không hợp lệ")]
        public int TotalRooms { get; set; } = 1;

        // Customer Info
        [Required(ErrorMessage = "Vui lòng nhập họ và tên người đặt")]
        [StringLength(100)]
        [Display(Name = "Họ và tên")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        [Display(Name = "Email")]
        public string CustomerEmail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string CustomerPhone { get; set; } = string.Empty;

        [Display(Name = "Địa chỉ")]
        public string? CustomerAddress { get; set; }

        [Display(Name = "Yêu cầu đặc biệt (nếu có)")]
        public string? Notes { get; set; }

        // Payment Method Selection
        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        public string PaymentMethod { get; set; } = "VNPay"; // VNPay, MoMo, BankTransfer, Cash

        // Pricing Breakdown
        public int TotalNights => Math.Max(1, (CheckOutDate.Date - CheckInDate.Date).Days);
        public decimal UnitPrice { get; set; }
        public decimal SubTotal => UnitPrice * TotalNights * TotalRooms;
        public decimal TaxAmount => Math.Round(SubTotal * 0.08m, 0);
        public decimal ServiceFee => Math.Round(SubTotal * 0.05m, 0);
        public decimal TotalAmount => SubTotal + TaxAmount + ServiceFee;
    }

    public class PaymentDemoViewModel
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; } = string.Empty;
        public string HotelName { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = "VNPay";
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string QrImageUrl { get; set; } = string.Empty;
    }

    public class BookingSuccessViewModel
    {
        public Booking Booking { get; set; } = null!;
    }
}
