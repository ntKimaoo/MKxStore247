using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MKxStore247.Models
{
    public class ProductOption
    {
        [Key]
        public int OptionId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(100)]
        public string OptionName { get; set; } // Ví d?: "Màu s?c", "Size"

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        public virtual ICollection<ProductOptionValue> Values { get; set; } = new List<ProductOptionValue>();
    }
}
