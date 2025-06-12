using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MKxStore247.Models
{
    public class ProductRating
    {
        [Key]
        public int RatingId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Stars { get; set; } // S? l??ng sao (1-5)

        [MaxLength(1000)]
        public string Comment { get; set; }

        [Required]
        public string UserId { get; set; } // Ng??i ?ánh giá

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }

        [ForeignKey("UserId")]
        public virtual UserApplication User { get; set; }
    }
}
