namespace MKxStore247.Models
{
    public class Address
    {
        public int AddressId { get; set; }
        public string UserId { get; set; }
        public string AddressLine { get; set; }
        public string District { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; } = "Việt Nam";
        public bool IsDefault { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public virtual UserApplication User { get; set; }
    }

}
