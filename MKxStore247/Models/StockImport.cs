using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MKxStore247.Models
{
    public class StockImport
    {
        [Key]
        public int ImportId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [MaxLength(255)]
        public string Note { get; set; }

        public DateTime ImportedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }

}
