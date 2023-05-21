using System;

namespace HyperPost.DTO.Package
{
    public class PackageResponse
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
        public decimal PackagePrice { get; set; }
        public decimal DeliveryPrice { get; set; }
        public decimal Weight { get; set; }
        public string? Description { get; set; }
    }
}
