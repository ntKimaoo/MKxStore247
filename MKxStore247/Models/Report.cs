using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MKxStore247.Models
{
    public enum ReportStatus
    {
        Pending = 0,
        Processed = 1,
        Rejected = 2
    }

    public class Report
    {
        [Key]
        public int ReportId { get; set; }

        [Required]
        [MaxLength(450)]
        public string ReporterUserId { get; set; }  // Người báo cáo

        public int? ProductId { get; set; }        // Có thể báo cáo sản phẩm

        public int? ShopId { get; set; }           // Hoặc báo cáo cửa hàng

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; }         // Lý do báo cáo

        [MaxLength(2000)]
        public string Description { get; set; }

        public ReportStatus Status { get; set; } = ReportStatus.Pending;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessedAt { get; set; }

        [ForeignKey("ReporterUserId")]
        public virtual UserApplication ReporterUser { get; set; }

        [ForeignKey("ProductId")]
        public virtual Product ReportedProduct { get; set; }

        [ForeignKey("ShopId")]
        public virtual Shop ReportedShop { get; set; }
    }

}
