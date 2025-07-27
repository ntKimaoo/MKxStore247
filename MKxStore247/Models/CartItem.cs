namespace MKxStore247.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; } 
        public int Quantity { get; set; }
        public virtual Cart Cart { get; set; } 
        public virtual Product Product { get; set; }
        public virtual ICollection<CartItemOption> SelectedOptions { get; set; } = new List<CartItemOption>();
    }

}
