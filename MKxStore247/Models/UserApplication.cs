using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MKxStore247.Models;

// Add profile data for application users by adding properties to the UserApplication class
public class UserApplication : IdentityUser
{
    public string FullName { get; set; }

    public string? AvatarUrl { get; set; } 

    public string? Gender { get; set; } 

    public DateTime? DateOfBirth { get; set; }

    public string? Address { get; set; }

    public DateTime? LastLoginAt { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; } = false;

    public string CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public string? UpdatedBy { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public virtual ICollection<Shop> Shops { get; set; } = new List<Shop>();
    public virtual ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public virtual ICollection<Report> Reports { get; set; } = new List<Report>();
    public virtual ICollection<Order> Orders { get; set; }
    public virtual ICollection<Cart> Carts { get; set; } = new List<Cart>();
    public virtual ICollection<UserCoupon> UserCoupons { get; set; } = new List<UserCoupon>();
    public virtual ICollection<ProductWishlist> Wishlists { get; set; } = new List<ProductWishlist>();

}


