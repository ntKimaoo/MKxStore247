using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MKxStore247.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        public int ShopId { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string ProductName { get; set; }

        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }  

        public bool IsActive { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        [ForeignKey("ShopId")]
        public virtual Shop Shop { get; set; }
        [ForeignKey("CategoryId")]
        public virtual CategoryProduct Category { get; set; }
        [Required]
        public int StockQuantity { get; set; } = 0;  // Số lượng tồn kho hiện tại
        [Required]
        public int SoldQuantity { get; set; } = 0;   // Tổng số lượng đã bán
        [Required]
        public int TotalImported { get; set; } = 0;  // Tổng số đã nhập kho
        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public virtual ICollection<ProductOption> ProductOptions { get; set; } = new List<ProductOption>();

        public virtual ICollection<StockImport> StockImports { get; set; } = new List<StockImport>();
        public virtual ICollection<CartItem> CartItems { get; set; }= new List<CartItem>();
        public virtual ICollection<OrderDetail> OrderItems { get; set; } = new List<OrderDetail>();
        public virtual ICollection<ProductRating> Ratings { get; set; } = new List<ProductRating>();
        public virtual ICollection<ProductWishlist> Wishlists { get; set; } = new List<ProductWishlist>();


    }

}
