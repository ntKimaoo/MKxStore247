using System.ComponentModel.DataAnnotations;

namespace MKxStore247.Models
{
    public class CategoryProduct
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; }

        [MaxLength(255)]
        public string Description { get; set; }

        [MaxLength(255)]
        public string ImageUrl { get; set; } // Ảnh đại diện danh mục

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Product> Products { get; set; }
    }

}
