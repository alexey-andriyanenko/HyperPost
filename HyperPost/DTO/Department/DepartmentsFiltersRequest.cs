using HyperPost.DTO.Pagination;

namespace HyperPost.DTO.Department
{
    public class DepartmentsFiltersRequest : PaginationRequest
    {
        public string? Address { get; set; }
    }
}
