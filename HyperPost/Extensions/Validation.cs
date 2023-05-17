using FluentValidation;
using HyperPost.DTO.PackageCategory;
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
                IValidator<CreatePackageCategoryRequest>,
                CreatePackageCategoryRequestValidator
            >();
            services.AddScoped<
                IValidator<UpdatePackageCategoryRequest>,
                UpdatePackageCategoryRequestValidator
            >();
            services.AddScoped<IValidator<PackageRequest>, PackageRequestValidator>();
        }
    }
}
