namespace HyperPost.DTO.Package
{
    public class CreatePackageRequest
    {
        public int CategoryId { get; set; }
        public int SenderUserId { get; set; }
        public int ReceiverUserId { get; set; }
        public int SenderDepartmentId { get; set; }
        public int ReceiverDepartmentId { get; set; }
        public decimal PackagePrice { get; set; }
        public decimal DeliveryPrice { get; set; }
        public decimal Weight { get; set; }
        public string? Description { get; set; }
    }
}
