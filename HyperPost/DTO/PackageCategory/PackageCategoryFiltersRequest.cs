using HyperPost.DTO.Pagination;

namespace HyperPost.DTO.PackageCategory
{
    public class PackageCategoryFiltersRequest : PaginationRequest
    {
        public string? Name { get; set; }
    }
}
