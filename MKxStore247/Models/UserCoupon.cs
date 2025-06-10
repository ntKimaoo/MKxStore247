using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MKxStore247.Models
{
    public class UserCoupon
    {
        [Key]
        public int UserCouponId { get; set; }

        [Required]
        [MaxLength(450)]
        public string UserId { get; set; }

        [Required]
        public int CouponId { get; set; }

        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("UserId")]
        public virtual UserApplication User { get; set; }

        [ForeignKey("CouponId")]
        public virtual Coupon Coupon { get; set; }
    }
}
