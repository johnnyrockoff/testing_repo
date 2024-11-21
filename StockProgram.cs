using System.Linq.Expressions;
using System.Reflection;

namespace InventoryTask;

class StockProgram
{
    /**
     * Used reflection to create the Linq query dynamically since the ordering type was parameterized
     */
    
    private class Product
    {
        public string Name { get; set; }
        public int Price { get; set; }
        public int Stock { get; set; }
    }

    private static List<Product> BuildList() => new()
    {
        { new(){ Name="Product A", Price=100, Stock=5}},
        { new(){ Name="Product C", Price=200, Stock=3}},
        { new(){ Name="Product B", Price=50, Stock=10}},
    };
    
    static void Main(string[] args)
    {
        Console.WriteLine("#######");
        ordering_stock_by("stock", "asc");
        Console.WriteLine("#######");
        ordering_stock_by("stock", "desc");
        Console.WriteLine("#######");
        ordering_stock_by("name", "asc");
        Console.WriteLine("#######");
        ordering_stock_by("price", "asc");
        Console.WriteLine("#######");
    }
    
    private static void ordering_stock_by(string _orderBy, string _order)
    {
        List<Product> products = BuildList();
        
        string command = _order.ToLower() == "desc" ? "OrderByDescending" : "OrderBy";
        var parameter = Expression.Parameter(typeof(Product), "p");
        
        var property = Expression.Property(parameter, _orderBy);
        
        var lambda = Expression.Lambda(property, parameter);
        
        var method = typeof(Enumerable)
            .GetMethods()
            .First(m => m.Name == command && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(Product), property.Type);
        
        var orderedProducts = (IEnumerable<Product>)method.Invoke(null, new object[] { products, lambda.Compile() })!;
        
        foreach (var item in orderedProducts)
        {
            Console.WriteLine($"{item.Name} - Price: {item.Price}, Stock: {item.Stock}");
        }
    }
}