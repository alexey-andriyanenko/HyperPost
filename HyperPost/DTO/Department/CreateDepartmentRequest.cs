using System.ComponentModel.DataAnnotations;

namespace HyperPost.DTO.Department
{
    public class CreateDepartmentRequest
    {
        public int Number { get; set; }
        public string FullAddress { get; set; }
    }
}
