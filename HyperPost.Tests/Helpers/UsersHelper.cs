using HyperPost.DB;
using HyperPost.DTO.User;
using HyperPost.Models;
using HyperPost.Shared;
using System.Net.Http.Json;

namespace HyperPost.Tests.Helpers
{
    public static class UsersHelper
    {
        public static async Task LoginAs(UserRolesEnum role, HttpClient client)
        {
            var user = _GetUserRequestData(role);
            var response = await client.PostAsJsonAsync
        }

        private static UserModel _GetUserModel(UserRolesEnum role)
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
                user.Email = "manager@example.co";
                user.Password = "manager_passwor";
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

        private static UserLoginViaPhoneNumberRequest _GetUserLoginViaPhoneNumbertData(UserRolesEnum role)
        {
            var user = _GetUserModel(role);
            var login = new UserLoginViaPhoneNumberRequest
            {
                PhoneNumber = user.PhoneNumber,
                Password = user.Password
            };

            return login;
        }
    }
}
