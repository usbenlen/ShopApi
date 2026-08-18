namespace Shop.Application.Constants;

public static class CacheKeys
{
    private const string CategoriesPrefix = "categories";
    private const string ProductsPrefix = "products";

    // -- Categories --

    public const string CategoriesGroup = CategoriesPrefix;

    public const string AllCategories = $"{CategoriesPrefix}:all";
    public static string Category(int id) => $"{CategoriesPrefix}:{id}";
    public static string CategoryTree(int id) => $"{CategoriesPrefix}:{id}:tree";
    public static string ParentCategories(int id) => $"{CategoriesPrefix}:{id}:parents";
    public static string ChildCategories(int id) => $"{CategoriesPrefix}:{id}:children";

    // -- Products --

    public const string ProductsGroup = ProductsPrefix;

    public const string AllProducts = $"{ProductsPrefix}:all";
    public static string Product(int id) => $"{ProductsPrefix}:{id}";
}
