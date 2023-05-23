using Microsoft.AspNetCore.Mvc;

namespace HyperPost.DTO.Pagination
{
    public class PaginationRequest
    {
        [FromQuery(Name = "page")]
        public int Page { get; set; }

        [FromQuery(Name = "limit")]
        public int Limit { get; set; }
    }
}
