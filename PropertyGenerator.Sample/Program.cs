using System;
using PropertyGenerator;

namespace PropertyGeneratorSample
{
    public partial class Product
    {
        [GetterProperty(PropertyName = "Identifier")]
        private readonly int id;

        [GetterProperty] private readonly string name;

        public Product(string name, int id)
        {
            this.name = name;
            this.id = id;
        }
    }

    public static class UseAutoNotifyGenerator
    {
        public static void Main()
        {
            var product = new Product("Great Pen", 100);

            Console.WriteLine($"Product.Name = {product.Name}");
            Console.WriteLine($"Product.Identifier = {product.Identifier}");
        }
    }
}