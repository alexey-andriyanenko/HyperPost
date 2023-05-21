using HyperPost.Models;

namespace HyperPost.Tests.Helpers
{
    public enum StatusesEnum
    {
        Created = 1,
        Sent = 2,
        Arrived = 3,
        Received = 4,
        Archived = 5
    }

    public static class StatusesHelper
    {
        public static PackageStatusModel GetPackageStatusModel(StatusesEnum statusType)
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
