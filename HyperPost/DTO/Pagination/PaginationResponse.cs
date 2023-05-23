using System.Collections.Generic;

namespace HyperPost.DTO.Pagination
{
    public class PaginationResponse<T>
        where T : class
    {
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<T> List { get; set; } = new List<T>();
    }
}
