using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MKxStore247.Models
{
    public class CartItemOption
    {
        [Key]
        public int CartItemOptionId { get; set; }

        [Required]
        public int CartItemId { get; set; }

        [Required]
        public int OptionId { get; set; }

        [Required]
        public int ValueId { get; set; }

        [ForeignKey("CartItemId")]
        public virtual CartItem CartItem { get; set; }

        [ForeignKey("OptionId")]
        public virtual ProductOption Option { get; set; }

        [ForeignKey("ValueId")]
        public virtual ProductOptionValue Value { get; set; }
    }
}
