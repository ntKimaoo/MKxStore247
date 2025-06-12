using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MKxStore247.Models
{
    public class ProductOptionValue
    {
        [Key]
        public int ValueId { get; set; }

        [Required]
        public int OptionId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Value { get; set; } // Ví dụ: "Đỏ", "Xanh", "L", "XL"

        public int StockQuantity { get; set; } = 0; // Số lượng tồn kho cho option value

        public bool IsOutOfStock => StockQuantity <= 0; // True nếu hết hàng

        [ForeignKey("OptionId")]
        public virtual ProductOption Option { get; set; }
    }
}
