using System.ComponentModel.DataAnnotations;

namespace HyperPost.DTO.User
{
    public class CreateUserRequest
    {
        public int RoleId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Email { get; set; }
        public string PhoneNumber { get; set; }
        public string? Password { get; set; }
    }
}
