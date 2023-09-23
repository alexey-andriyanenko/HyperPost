using HyperPost.DTO.Department;
using HyperPost.DTO.PackageCategory;
using HyperPost.DTO.User;
using System;

namespace HyperPost.DTO.Package
{
    public class PackageResponse
    {
        public Guid Id { get; set; }
        public int StatusId { get; set; }

        public PackageCategoryResponse? Category { get; set; }
        public UserResponse SenderUser { get; set; }
        public UserResponse ReceiverUser { get; set; }

        public DepartmentResponse? SenderDepartment { get; set; }
        public DepartmentResponse? ReceiverDepartment { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public DateTime? SentAt { get; set; }
        public DateTime? ArrivedAt { get; set; }
        public DateTime? ReceivedAt { get; set; }
        public DateTime? ArchivedAt { get; set; }
        public decimal PackagePrice { get; set; }
        public decimal DeliveryPrice { get; set; }
        public decimal Weight { get; set; }
        public string? Description { get; set; }
    }
}
