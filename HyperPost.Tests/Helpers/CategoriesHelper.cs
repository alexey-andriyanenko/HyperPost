using HyperPost.Models;

namespace HyperPost.Tests.Helpers
{
    public enum CategoriesEnum
    {
        Food = 1,
        Money = 2,
        Medicaments = 3,
        Accumulators = 4,
        SportsProducts = 5,
        Clothes = 6,
        Shoes = 7,
        Documents = 8,
        Books = 9,
        Computers = 10,
    }

    public static class CategoriesHelper
    {
        public static PackageCategoryModel GetPackageCategoryModel(CategoriesEnum categoryType)
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
