using HyperPost.Models;

namespace HyperPost.DTO.Department
{
    public static class DepartmentExtentensions
    {
        public static DepartmentResponse ToResponse(this DepartmentModel model)
        {
            return new DepartmentResponse
            {
                Id = model.Id,
                Number = model.Number,
                FullAddress = model.FullAddress,
            };
        }
    }
}
