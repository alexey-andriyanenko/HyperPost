using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace HyperPost.Models
{
    [Table("Package")]
    public class PackageModel
    {
        public Guid Id { get; set; }
        public int StatusId { get; set; }
        public int CategoryId { get; set; }
        public int SenderUserId { get; set; }
        public int ReceiverUserId { get; set; }
        public int SenderDepartmentId { get; set; }
        public int ReceiverDepartmentId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? ArrivedAt { get; set; }
        public DateTime? ReceivedAt { get; set; }

        [Column(TypeName = "decimal(8, 2)")]
        public decimal PackagePrice { get; set; }

        [Column(TypeName = "decimal(8, 2)")]
        public decimal DeliveryPrice { get; set; }

        [Column(TypeName = "decimal(4, 2)")]
        public decimal Weight { get; set; }
    }
}
