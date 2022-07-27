using ProductShop.Data;
using ProductShop.Dtos.Import;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using System.Collections.Generic;
using ProductShop.Models;
using System.Text;
using ProductShop.Dtos.Export;
using Microsoft.EntityFrameworkCore;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            ProductShopContext context = new ProductShopContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //string xml = File.ReadAllText("../../../Datasets/categories-products.xml");

            string result = GetUsersWithProducts(context);

            System.Console.WriteLine(result);
        }

        //1. Import Users
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Users");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportUserDto[]), xmlRoot);

            using StringReader reader = new StringReader(inputXml);

            ImportUserDto[] dtos = (ImportUserDto[])serializer.Deserialize(reader);

            User[] users = dtos
                                        .Select(dto => new User()
                                        {
                                            FirstName = dto.FirstName,
                                            LastName = dto.LastName,
                                            Age = dto.Age
                                        })
                                        .ToArray();

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Length}";
        }

        //2. Import Products
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Products");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportProductDto[]), xmlRoot);

            using StringReader reader = new StringReader(inputXml);

            ImportProductDto[] dtos = (ImportProductDto[])serializer.Deserialize(reader);

            Product[] products = dtos
                                    .Select(dto => new Product()
                                    {
                                        Name = dto.Name,
                                        Price = dto.Price,
                                        SellerId = dto.SellerId,
                                        BuyerId = dto.BuyerId
                                    })
                                    .ToArray();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        //3. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Categories");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportCategoryDto[]), xmlRoot);

            using StringReader reader = new StringReader(inputXml);

            ImportCategoryDto[] dtos = (ImportCategoryDto[])serializer.Deserialize(reader);

            List<Category> categories = new List<Category>();

            foreach (var dto in dtos)
            {
                if(dto.Name != null)
                {
                    categories.Add(new Category()
                    {
                        Name = dto.Name
                    });
                }
            }

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        //4. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("CategoryProducts");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportCategoryProductDto[]), xmlRoot);

            using StringReader reader = new StringReader(inputXml);

            ImportCategoryProductDto[] dtos = (ImportCategoryProductDto[])serializer.Deserialize(reader);

            List<CategoryProduct> categoryProducts = new List<CategoryProduct>();

            foreach (var dto in dtos)
            {
                if(!dto.CategoryId.HasValue || !dto.ProductId.HasValue)
                {
                    continue;
                }

                CategoryProduct cp = new CategoryProduct()
                {
                    CategoryId = (int)dto.CategoryId,
                    ProductId = (int)dto.ProductId
                };

                categoryProducts.Add(cp);
            }

            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        //5. Export Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            ExportProductInRangeDto[] dtos = context
                                                .Products
                                                .Where(p => p.Price >= 500 && p.Price <= 1000)
                                                .Select(p => new ExportProductInRangeDto()
                                                {
                                                    Name = p.Name,
                                                    Price = p.Price,
                                                    BuyerName = $"{p.Buyer.FirstName} {p.Buyer.LastName}"
                                                })
                                                .OrderBy(p => p.Price)
                                                .Take(10)
                                                .ToArray();

            return Serialize<ExportProductInRangeDto[]>("Products", dtos);

        }

        //6. Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            ExportUserWithProductsDto[] dtos = context
                                                .Users
                                                .Where(u => u.ProductsSold.Any())
                                                .Select(u => new ExportUserWithProductsDto()
                                                {
                                                    FirstName = u.FirstName,
                                                    LastName = u.LastName,
                                                    SoldProducts = u.ProductsSold
                                                                        .Select(p => new ExportSoldProductDto()
                                                                        {
                                                                            Name = p.Name,
                                                                            Price = p.Price
                                                                        })
                                                                        .ToArray()
                                                })
                                                .OrderBy(u => u.LastName)
                                                .ThenBy(u => u.FirstName)
                                                .Take(5)
                                                .ToArray();

            return Serialize<ExportUserWithProductsDto[]>("Users", dtos);
        }

        //7. Export Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            ExportCategoryDto[] dtos = context.Categories
                                            .Select(c => new ExportCategoryDto()
                                            {
                                                Name = c.Name,
                                                Count = c.CategoryProducts.Count,
                                                AveragePrice = c.CategoryProducts
                                                                    .Average(cp => cp.Product.Price),
                                                TotalRevenue = c.CategoryProducts
                                                                    .Sum(cp => cp.Product.Price)
                                            })
                                            .OrderByDescending(c => c.Count)
                                            .ThenBy(c => c.TotalRevenue)
                                            .ToArray();

            return Serialize<ExportCategoryDto[]>("Categories", dtos);
        }

        //8. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            ExportUsersAndProductsDto dto = new ExportUsersAndProductsDto()
            {
                Count = context.Users
                                .Where(u => u.ProductsSold.Any())
                                .Count(),
                Users = context.Users
                                .Where(u => u.ProductsSold.Any())
                                .Include(u => u.ProductsSold)
                                .ToArray()
                                .Select(u => new ExportUserAndProductsWithCountDto()
                                {
                                    FirstName = u.FirstName,
                                    LastName = u.LastName,
                                    Age = u.Age,
                                    SoldProducts = new ExportUserSoldProductsDto()
                                    {
                                        Count = u.ProductsSold.Count,
                                        Products = u.ProductsSold
                                                .Select(p => new ExportProductDto()
                                                {
                                                    Name = p.Name,
                                                    Price = p.Price
                                                })
                                                .OrderByDescending(p => p.Price)
                                                .ToArray()
                                    }
                                })
                                .OrderByDescending(u => u.SoldProducts.Count)
                                .Take(10)
                                .ToArray()
            };

            return Serialize<ExportUsersAndProductsDto>("Users", dto);
        }

        //Helper method
        private static string Serialize<T>(string rootName, T dto)
        {
            StringBuilder sb = new StringBuilder();

            XmlRootAttribute xmlRoot = new XmlRootAttribute(rootName);
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlSerializer serializer = new XmlSerializer(typeof(T), xmlRoot);

            using StringWriter writer = new StringWriter(sb);

            serializer.Serialize(writer, dto, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}