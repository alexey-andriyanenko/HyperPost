using HyperPost.Models;
using HyperPost.Shared;

namespace HyperPost.Tests.Helpers
{
    public static class PackageStatusesHelper
    {
        public static PackageStatusModel GetPackageStatusModel(PackageStatusesEnum statusType)
        {
            var status = new PackageStatusModel { Id = (int)statusType, };
            switch ((int)statusType)
            {
                case 1:
                    status.Name = "created";
                    break;
                case 2:
                    status.Name = "sent";
                    break;
                case 3:
                    status.Name = "arrived";
                    break;
                case 4:
                    status.Name = "received";
                    break;
                case 5:
                    status.Name = "archived";
                    break;
            }
            return status;
        }
    }
}
