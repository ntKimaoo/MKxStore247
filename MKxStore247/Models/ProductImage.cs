using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MKxStore247.Models
{
    public class ProductImage
    {
        [Key]
        public int ImageId { get; set; }

        [Required]
        public int ProductId { get; set; }
        public string FileName => "From Main";

        [Required]
        [MaxLength(255)]
        public string ImageUrl { get; set; }

        public bool IsMain { get; set; } = false;

        public bool IsActive { get; set; } = true;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; }
    }
}