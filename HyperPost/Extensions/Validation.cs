using FluentValidation;
using HyperPost.DTO.Category;
using HyperPost.DTO.Department;
using HyperPost.DTO.Package;
using HyperPost.DTO.User;
using Microsoft.Extensions.DependencyInjection;

namespace HyperPost.Extensions
{
    public static class Validation
    {
        public static void AddHyperPostValidation(this IServiceCollection services)
        {
            services.AddScoped<IValidator<UserRequest>, UserRequestValidator>();
            services.AddScoped<IValidator<DepartmentRequest>, DepartmentRequestValidator>();
            services.AddScoped<
                IValidator<PackageCategoryRequest>,
                PackageCategoryRequestValidator
            >();
            services.AddScoped<IValidator<PackageRequest>, PackageRequestValidator>();
        }
    }
}
