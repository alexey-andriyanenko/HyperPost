using FluentValidation;
using HyperPost.DTO.Pagination;

namespace HyperPost.DTO.Department
{
    public class DepartmentsFiltersRequestValidator : AbstractValidator<DepartmentsFiltersRequest>
    {
        public DepartmentsFiltersRequestValidator()
        {
            Include(new PaginationRequestValidator());
        }
    }
}
