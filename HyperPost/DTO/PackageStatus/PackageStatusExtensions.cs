using HyperPost.Models;

namespace HyperPost.DTO.PackageStatus
{
    public static class PackageStatusExtensions
    {
        public static PackageStatusResponse ToResponse(this PackageStatusModel model)
        {
            return new PackageStatusResponse { Id = model.Id, Name = model.Name, };
        }
    }
}
