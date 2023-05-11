using System.ComponentModel.DataAnnotations;

namespace HyperPost.DTO.Department
{
    public class DepartmentRequest
    {
        public int Number { get; set; }
        public string FullAddress { get; set; }
    }
}
