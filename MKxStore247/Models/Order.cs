using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MKxStore247.Models
{
    public enum OrderStatus
    {
        Pending = 0,           // 🕓 Đã đặt hàng (chưa xử lý)
        Confirmed = 1,         // ✅ Đã xác nhận đơn hàng
        Processing = 2,        // 🛠️ Đang xử lý (đóng gói, chuẩn bị hàng)
        Shipped = 3,           // 🚚 Đã giao cho đơn vị vận chuyển
        InTransit = 4,         // 🚛 Đang giao
        Delivered = 5,         // 📦 Đã giao thành công
        Completed = 6,         // 🎉 Giao hàng xong + người dùng xác nhận
        Cancelled = 7,         // ❌ Bị hủy (người dùng hoặc shop hủy)
        Failed = 8,            // ⚠️ Giao hàng thất bại
        Returned = 9,          // ↩️ Hàng bị trả lại
        Refunded = 10          // 💸 Đã hoàn tiền
    }

    public class Order
    {
        [Key]
        public int OrderId { get; set; }

        [Required]
        [MaxLength(450)]
        public string UserId { get; set; }

        public int? AddressId { get; set; }

        public int? CouponId { get; set; }

        public int PaymentMethodId { get; set; }

        public decimal TmpAmount { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual UserApplication User { get; set; }

        [ForeignKey("AddressId")]
        public virtual Address Address { get; set; }

        [ForeignKey("CouponId")]
        public virtual Coupon Coupon { get; set; }

        [ForeignKey("PaymentMethodId")]
        public virtual PaymentMethod PaymentMethod { get; set; }

        public virtual ICollection<OrderDetail> Items { get; set; }
    }
}
