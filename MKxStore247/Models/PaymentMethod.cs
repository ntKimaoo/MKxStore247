namespace MKxStore247.Models
{
    public class PaymentMethod
    {
        public int PaymentMethodId { get; set; }
        public string Name { get; set; } // "COD", "VNPAY", "MOMO", ...
        public string Description { get; set; }
        public Boolean IsActive { get; set; } = true;
        public virtual ICollection<Order> Orders { get; set; }
    }

}
