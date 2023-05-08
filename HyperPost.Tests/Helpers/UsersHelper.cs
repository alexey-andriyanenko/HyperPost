using HyperPost.DB;
using HyperPost.DTO.User;

namespace HyperPost.Tests.Helpers
{
    public enum UsersEnum
    {
        Admin,
        Manager,
        Client,
    }
    public static class UsersHelper
    {
        public static UserRequest GetUserRequest(UsersEnum variant)
        {
            var user = new UserRequest();

            if (variant == UsersEnum.Admin)
            {
                user.RoleId = 1;
                user.FirstName = "Admin";
                user.LastName = "User";
                user.PhoneNumber = "+380975523355";
                user.Email = "admin@example.com";
                user.Password = "root";
            }

            if (variant == UsersEnum.Manager)
            {
                user.RoleId = 2;
                user.FirstName = "Manager";
                user.LastName = "User";
                user.PhoneNumber = "+380659327431";
                user.Email = "manager@example.com";
                user.Password = "manager_password";
            }

            if (variant == UsersEnum.Client)
            {
                user.RoleId = 3;
                user.FirstName = "Client";
                user.LastName = "User";
                user.PhoneNumber = "+380986027902";
                user.Email = "client@example.com";
                user.Password = "client_password";
            }

            return user;
        }

        public static async Task<UserLogin> CreateAndLoginUser(UsersEnum variant, HttpClient httpClient)
        {
            var user = GetUserRequest(variant);
            var response = await httpClient
        }
    }
}
