using HyperPost.Models;

namespace HyperPost.DTO.PackageCategory
{
    public static class PackageCategoryExtensions
    {
        public static PackageCategoryResponse ToResponse(this PackageCategoryModel model)
        {
            return new PackageCategoryResponse { Id = model.Id, Name = model.Name, };
        }
    }
}
