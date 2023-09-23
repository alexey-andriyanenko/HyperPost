using HyperPost.Models;

namespace HyperPost.DTO.User
{
    public static class UserExtensions
    {
        public static UserResponse ToResponse(this UserModel model)
        {
            return new UserResponse
            {
                Id = model.Id,
                RoleId = model.RoleId,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
            };
        }
    }
}
