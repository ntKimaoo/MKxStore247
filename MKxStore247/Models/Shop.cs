namespace MKxStore247.Models
{
    public class Shop
    {
        public int ShopId { get; set; }
        public string ShopUrl { get; set; }
        public string OwnerId { get; set; }
        public string ShopName { get; set; }
        public string? Description { get; set; }
        public string? Avatar { get; set; }
        public string? CoverImage { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public int Status { get; set; } = 1;
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public virtual UserApplication Owner { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
