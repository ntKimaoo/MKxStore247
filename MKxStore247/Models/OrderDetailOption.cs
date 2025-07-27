using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MKxStore247.Models
{
    public class OrderDetailOption
    {
        [Key]
        public int OrderDetailOptionId { get; set; }

        [Required]
        public int OrderDetailId { get; set; }

        [Required]
        public int OptionId { get; set; }

        [Required]
        public int ValueId { get; set; }

        [Required]
        [MaxLength(100)]
        public string OptionName { get; set; }

        [Required]
        [MaxLength(100)]
        public string OptionValue { get; set; }

        [ForeignKey("OrderDetailId")]
        public virtual OrderDetail OrderDetail { get; set; }

        [ForeignKey("OptionId")]
        public virtual ProductOption Option { get; set; }

        [ForeignKey("ValueId")]
        public virtual ProductOptionValue Value { get; set; }
    }
}
