using FluentValidation;
using HyperPost.DTO.Pagination;

namespace HyperPost.DTO.PackageCategory
{
    public class PackageCategoryFiltersRequestValidator
        : AbstractValidator<PackageCategoryFiltersRequest>
    {
        public PackageCategoryFiltersRequestValidator()
        {
            Include(new PaginationRequestValidator());
        }
    }
}
