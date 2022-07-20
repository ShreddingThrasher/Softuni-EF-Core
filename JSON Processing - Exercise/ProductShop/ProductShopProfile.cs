using AutoMapper;
using ProductShop.DTOs.Category;
using ProductShop.DTOs.Product;
using ProductShop.DTOs.User;
using ProductShop.Models;
using ProductShop.DTOs.CategoryProduct;
using System.Linq;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            //imports
            this.CreateMap<ImportUserDto, User>();
            this.CreateMap<ImportProductDto, Product>();
            this.CreateMap<ImportCategoryDto, Category>();
            this.CreateMap<ImportCategoryProductDto, CategoryProduct>();


            //exports
            this.CreateMap<Product, ExportProductsInRangeDto>()
                .ForMember(d => d.SellerFullName, 
                           mo => mo.MapFrom(s => $"{s.Seller.FirstName} {s.Seller.LastName}"));


            //problem 6
            //inner DTO
            this.CreateMap<Product, ExportUserSoldProductsDto>()
                .ForMember(d => d.BuyerFirstName,
                           mo => mo.MapFrom(s => s.Buyer.FirstName))
                .ForMember(d => d.BuyerLastName,
                           mo => mo.MapFrom(s => s.Buyer.LastName));

            //Outer DTO
            this.CreateMap<User, ExportUsersWithSoldProductsDto>()
                .ForMember(d => d.SoldProducts,
                           mo => mo.MapFrom(s => s.ProductsSold
                                                    .Where(p => p.BuyerId.HasValue)));

            //problem 7 DTO
            this.CreateMap<Category, ExportCategoryByProductsCountDto>()
                .ForMember(d => d.ProductsCount,
                           mo => mo.MapFrom(s => s.CategoryProducts.Count))
                .ForMember(d => d.AveragePrice,
                           mo => mo.MapFrom(s => s.CategoryProducts
                                                    .Average(cp => cp.Product.Price).ToString("F2")))
                .ForMember(d => d.TotalRevenue,
                           mo => mo.MapFrom(s => s.CategoryProducts
                                                    .Sum(cp => cp.Product.Price).ToString("F2")));


            //problem 8 DTOs

            this.CreateMap<Product, ExportSoldProductShortInfoDto>();

            this.CreateMap<User, ExportSoldProductsFullInfoDto>()
                .ForMember(d => d.SoldProducts,
                           mo => mo.MapFrom(s => s.ProductsSold
                                                    .Where(p => p.BuyerId.HasValue)));

            this.CreateMap<User, ExportUsersWithFullProductInfoDto>()
                .ForMember(d => d.SoldProductsInfo,
                    mo => mo.MapFrom(s => s));
        }
    }
}
