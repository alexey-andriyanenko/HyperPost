using HyperPost.Models;

namespace HyperPost.Tests.Helpers
{
    public enum PackageCategoriesEnum
    {
        NonExistent,
        Food,
        Money,
        Medicaments,
        Accumulators,
        SportsProducts,
        Clothes,
        Shoes,
        Documents,
        Books,
        Computers,
    }

    public static class PackageCategoriesHelper
    {
        public static PackageCategoryModel GetPackageCategoryModel(
            PackageCategoriesEnum categoryType
        )
        {
            var category = new PackageCategoryModel { Id = (int)categoryType, };

            switch ((int)categoryType)
            {
                case 1:
                    category.Name = "Food";
                    break;
                case 2:
                    category.Name = "Money";
                    break;
                case 3:
                    category.Name = "Medicaments";
                    break;
                case 4:
                    category.Name = "Accumulators";
                    break;
                case 5:
                    category.Name = "Sports Products";
                    break;
                case 6:
                    category.Name = "Clothes";
                    break;
                case 7:
                    category.Name = "Shoes";
                    break;
                case 8:
                    category.Name = "Documents";
                    break;
                case 9:
                    category.Name = "Books";
                    break;
                case 10:
                    category.Name = "Computers";
                    break;
            }

            return category;
        }
    }
}
