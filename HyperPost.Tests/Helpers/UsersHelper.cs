using HyperPost.DTO.User;
using HyperPost.Models;
using HyperPost.Shared;
using System.Net.Http.Json;

namespace HyperPost.Tests.Helpers
{
    public enum TestUsersEnum
    {
        Admin,
        Manager,
        DefaultClient,
        SenderClient,
        ReceiverClient
    }

    public static class UsersHelper
    {
        public static async Task<UserLoginResponse> LoginViaEmailAs(
            this HttpClient client,
            UserRolesEnum role
        )
        {
            var login = _GetUserLoginViaEmailData(role);
            var response = await client.PostAsJsonAsync("/users/login/email", login);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<UserLoginResponse>();
            Assert.NotNull(content);

            return content;
        }

        public static async Task<UserLoginResponse> LoginViaPhoneNumberAs(
            this HttpClient client,
            UserRolesEnum role
        )
        {
            var login = _GetUserLoginViaPhoneNumbertData(role);
            var response = await client.PostAsJsonAsync("/users/login/phone", login);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadFromJsonAsync<UserLoginResponse>();
            Assert.NotNull(content);

            return content;
        }

        public static UserModel GetExistingUserModel(UserRolesEnum role)
        {
            var user = new UserModel();

            if (role == UserRolesEnum.Admin)
            {
                user.Id = (int)role;
                user.RoleId = (int)role;
                user.FirstName = "Admin";
                user.LastName = "User";
                user.PhoneNumber = "111111";
                user.Email = "admin@example.com";
                user.Password = "root";
            }

            if (role == UserRolesEnum.Manager)
            {
                user.Id = (int)role;
                user.RoleId = (int)role;
                user.FirstName = "Manager";
                user.LastName = "User";
                user.PhoneNumber = "222222";
                user.Email = "manager@example.com";
                user.Password = "manager_password";
            }

            if (role == UserRolesEnum.Client)
            {
                user.Id = (int)role;
                user.RoleId = (int)role;
                user.FirstName = "Client";
                user.LastName = "User";
                user.PhoneNumber = "333333";
                user.Email = "client@example.com";
                user.Password = "client_password";
            }

            return user;
        }

        public static CreateUserRequest GetUserRequest(TestUsersEnum testUser)
        {
            var user = new CreateUserRequest();

            if (testUser == TestUsersEnum.DefaultClient)
            {
                user.RoleId = (int)UserRolesEnum.Client;
                user.FirstName = "Client";
                user.LastName = "User";
                user.PhoneNumber = "444444";
                user.Email = "default@example.com";
            }

            if (testUser == TestUsersEnum.SenderClient)
            {
                user.RoleId = (int)UserRolesEnum.Client;
                user.FirstName = "Client";
                user.LastName = "User";
                user.PhoneNumber = "555555";
                user.Email = "sender@email.com";
            }

            if (testUser == TestUsersEnum.ReceiverClient)
            {
                user.RoleId = (int)UserRolesEnum.Client;
                user.FirstName = "Client";
                user.LastName = "User";
                user.PhoneNumber = "666666";
                user.Email = "receiver@example.com";
            }

            if (testUser == TestUsersEnum.Admin)
            {
                user.RoleId = (int)UserRolesEnum.Admin;
                user.FirstName = "Admin";
                user.LastName = "User";
                user.PhoneNumber = "777777";
                user.Email = "test_admin@example.com";
                user.Password = "test_admin";
            }

            if (testUser == TestUsersEnum.Manager)
            {
                user.RoleId = (int)UserRolesEnum.Manager;
                user.FirstName = "Manager";
                user.LastName = "User";
                user.PhoneNumber = "888888";
                user.Email = "test_manager@example.com";
                user.Password = "test_manager";
            }

            return user;
        }

        private static UserLoginViaPhoneNumberRequest _GetUserLoginViaPhoneNumbertData(
            UserRolesEnum role
        )
        {
            var user = GetExistingUserModel(role);
            var login = new UserLoginViaPhoneNumberRequest
            {
                PhoneNumber = user.PhoneNumber,
                Password = user.Password
            };

            return login;
        }

        private static UserLoginViaEmailRequest _GetUserLoginViaEmailData(UserRolesEnum role)
        {
            var user = GetExistingUserModel(role);
            var login = new UserLoginViaEmailRequest
            {
                Email = user.Email,
                Password = user.Password
            };

            return login;
        }
    }
}
