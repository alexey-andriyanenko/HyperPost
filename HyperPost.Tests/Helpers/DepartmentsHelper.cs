using HyperPost.Models;

namespace HyperPost.Tests.Helpers
{
    public static class DepartmentsHelper
    {
        public static DepartmentModel GetDepartmentModel(int number)
        {
            var department = new DepartmentModel { Id = number, Number = number, };

            switch (number)
            {
                case 1:
                    department.FullAddress =
                        "HyperPost Department #1, 5331 Rexford Court, Montgomery AL 36116";
                    break;
                case 2:
                    department.FullAddress =
                        "HyperPost Department #2, 6095 Terry Lane, Golden CO 80403";
                    break;
                case 3:
                    department.FullAddress =
                        "HyperPost Department #3, 1002 Hilltop Drive, Dalton GA 30720";
                    break;
                case 4:
                    department.FullAddress =
                        "HyperPost Department #4, 2325 Eastridge Circle, Moore OK 73160";
                    break;
                case 5:
                    department.FullAddress =
                        "HyperPost Department #5, 100219141 Pine Ridge Circle, Anchorage AK 99516";
                    break;
                case 6:
                    department.FullAddress =
                        "HyperPost Department #6, 5275 North 59th Avenue, Glendale AZ 85301";
                    break;
                case 7:
                    department.FullAddress =
                        "HyperPost Department #7, 5985 Lamar Street, Arvada CO 80003";
                    break;
                case 8:
                    department.FullAddress =
                        "HyperPost Department #8, 136 Acacia Drive, Blue Lake CA 95525";
                    break;
                case 9:
                    department.FullAddress =
                        "HyperPost Department #9, 7701 Taylor Oaks Circle, Montgomery AL 36116";
                    break;
                case 10:
                    department.FullAddress =
                        "HyperPost Department #10, 243 Kentucky Avenue, Pasadena MD 21122";
                    break;
            }

            return department;
        }
    }
}
