using HyperPost.DTO.Department;
using HyperPost.DTO.PackageCategory;
using HyperPost.DTO.User;
using HyperPost.Models;

namespace HyperPost.DTO.Package
{
    public static class PackageExtensions
    {
        public static PackageResponse ToResponse(this PackageModel model)
        {
            return new PackageResponse
            {
                Id = model.Id,
                Category = model.Category?.ToResponse(),
                SenderDepartment = model.SenderDepartment?.ToResponse(),
                ReceiverDepartment = model.ReceiverDepartment?.ToResponse(),
                SenderUser = model.SenderUser.ToResponse(),
                ReceiverUser = model.ReceiverUser.ToResponse(),
                StatusId = model.StatusId,
                CreatedAt = model.CreatedAt,
                ModifiedAt = model.ModifiedAt,
                SentAt = model.SentAt,
                ArrivedAt = model.ArrivedAt,
                ReceivedAt = model.ReceivedAt,
                ArchivedAt = model.ArchivedAt,
                PackagePrice = model.PackagePrice,
                DeliveryPrice = model.DeliveryPrice,
                Weight = model.Weight,
                Description = model.Description
            };
        }
    }
}
