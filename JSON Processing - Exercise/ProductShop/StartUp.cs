using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.Models;
using ProductShop.DTOs.User;
using AutoMapper;
using ProductShop.DTOs.Product;
using System.ComponentModel.DataAnnotations;
using ProductShop.DTOs.Category;
using ProductShop.DTOs.CategoryProduct;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile(typeof(ProductShopProfile)));

            using ProductShopContext context = new ProductShopContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //Console.WriteLine("Database copy was created!");

            //string json = File.ReadAllText("../../../Datasets/categories-products.json");

            //string msg = ImportCategoryProducts(context, json);

            string output = GetUsersWithProducts(context);

            Console.WriteLine(output);
        }


        //1. Import users
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            ImportUserDto[] userDtos = JsonConvert.DeserializeObject<ImportUserDto[]>(inputJson);

            ICollection<User> users = new List<User>();

            foreach (var dto in userDtos)
            {
                if (!IsValid(dto))
                {
                    continue;
                }

                User user = Mapper.Map<User>(dto);

                users.Add(user);
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        //2. Import Products
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            ImportProductDto[] productDtos = JsonConvert.DeserializeObject<ImportProductDto[]>(inputJson);

            ICollection<Product> products = new List<Product>();

            foreach (var dto in productDtos)
            {
                if (!IsValid(dto))
                {
                    continue;
                }

                Product product = Mapper.Map<Product>(dto);

                products.Add(product);
            }

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        //3. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            ImportCategoryDto[] categoryDtos = JsonConvert
                .DeserializeObject<ImportCategoryDto[]>(inputJson);

            ICollection<Category> validCategories = new List<Category>();

            foreach (var cDto in categoryDtos)
            {
                if (!IsValid(cDto))
                {
                    continue;
                }

                Category category = Mapper.Map<Category>(cDto);
                validCategories.Add(category);
            }

            context.Categories.AddRange(validCategories);
            context.SaveChanges();

            return $"Successfully imported {validCategories.Count}";
        }

        //4. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            ImportCategoryProductDto[] cpDtos = JsonConvert
                .DeserializeObject<ImportCategoryProductDto[]>(inputJson);

            ICollection<CategoryProduct> cps = new List<CategoryProduct>();

            foreach (var cpDto in cpDtos)
            {
                CategoryProduct cp = Mapper.Map<CategoryProduct>(cpDto);
                cps.Add(cp);
            }

            context.CategoryProducts.AddRange(cps);
            context.SaveChanges();

            return $"Successfully imported {cps.Count}";
        }

        //5. Export Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            ExportProductsInRangeDto[] products = context.Products
                            .Where(p => p.Price >= 500 && p.Price <= 1000)
                            .OrderBy(p => p.Price)
                            .ProjectTo<ExportProductsInRangeDto>()
                            .ToArray();

            return JsonConvert.SerializeObject(products, Formatting.Indented);
        }

        //6. Export sold products
        public static string GetSoldProducts(ProductShopContext context)
        {
            ExportUsersWithSoldProductsDto[] data = context.Users
                        .Include(u => u.ProductsSold)
                        .Where(u => u.ProductsSold.Any(p => p.BuyerId.HasValue))
                        .OrderBy(u => u.LastName)
                        .ThenBy(u => u.FirstName)
                        .ProjectTo<ExportUsersWithSoldProductsDto>()
                        .ToArray();

            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }

        //7. Export Categories by Products Count (Solver using anonymous object)
        public static string GetCategoriesByProductsCountWithoutDto(ProductShopContext context)
        {
            var data = context.Categories
                        .OrderByDescending(c => c.CategoryProducts.Count)
                        .Select(c => new
                        {
                            category = c.Name,
                            productsCount = c.CategoryProducts.Count,
                            averagePrice = c.CategoryProducts.Average(cp => cp.Product.Price).ToString("F2"),
                            totalRevenue = c.CategoryProducts
                                                .Sum(cp => cp.Product.Price).ToString("F2")
                        })
                        .ToArray();

            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }

        //7. Export Categories by Products Count (Solved using DTO)
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            ExportCategoryByProductsCountDto[] dtos = context.Categories
                                                    .OrderByDescending(c => c.CategoryProducts.Count)
                                                    .ProjectTo<ExportCategoryByProductsCountDto>()
                                                    .ToArray();

            return JsonConvert.SerializeObject(dtos, Formatting.Indented);
        }

        //8. Export Users and Products (Solved with AutoMapper)
        public static string GetUsersWithProductsAutoMapper(ProductShopContext context)
        {
            ExportUsersWithFullProductInfoDto[] users = context.Users
                            .Where(u => u.ProductsSold.Any(p => p.BuyerId.HasValue))
                            .OrderByDescending(u => u.ProductsSold.Count(p => p.BuyerId.HasValue))
                            .ProjectTo<ExportUsersWithFullProductInfoDto>()
                            .ToArray();

            ExportUsersInfoDto mainDto = new ExportUsersInfoDto()
            {
                UsersCount = users.Length,
                Users = users
            };

            JsonSerializerSettings settings = new JsonSerializerSettings();

            settings.NullValueHandling = NullValueHandling.Ignore;

            return JsonConvert.SerializeObject(mainDto, Formatting.Indented, settings);
        }

        //8. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                            .Include(u => u.ProductsSold)
                            .ToArray()
                            .Where(u => u.ProductsSold.Any(p => p.BuyerId.HasValue))
                            .OrderByDescending(u => u.ProductsSold.Count(p => p.BuyerId.HasValue))
                            .Select(u => new
                            {
                                firstName = u.FirstName,
                                lastName = u.LastName,
                                age = u.Age,
                                soldProducts = new
                                {
                                    count = u.ProductsSold.Count(p => p.BuyerId.HasValue),
                                    products = u.ProductsSold
                                                    .Where(p => p.BuyerId.HasValue)
                                                    .Select(p => new
                                                    {
                                                        name = p.Name,
                                                        price = p.Price
                                                    })
                                                    .ToArray()
                                }
                            })
                            .ToArray();

            object obj = new
            {
                usersCount = users.Count(),
                users = users
            };

            JsonSerializerSettings settings = new JsonSerializerSettings();

            settings.NullValueHandling = NullValueHandling.Ignore;

            return JsonConvert.SerializeObject(obj, Formatting.Indented, settings);
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}