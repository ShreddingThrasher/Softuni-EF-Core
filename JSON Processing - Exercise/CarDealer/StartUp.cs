using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using AutoMapper;
using CarDealer.Data;
using CarDealer.DTO.Car;
using CarDealer.DTO.Customer;
using CarDealer.DTO.Part;
using CarDealer.DTO.Sale;
using CarDealer.DTO.Supplier;
using CarDealer.Models;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            Mapper.Initialize(cfg => cfg.AddProfile(typeof(CarDealerProfile)));
            CarDealerContext context = new CarDealerContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //string jsonSuppliers = File.ReadAllText(@"../../../Datasets/suppliers.json");
            //string json = File.ReadAllText(@"../../../Datasets/sales.json");

            //string output = ImportSuppliers(context, jsonSuppliers);
            string output = GetSalesWithAppliedDiscount(context);


            Console.WriteLine(output);
            //Console.WriteLine(output);
        }

        //9. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliersDtos = JsonConvert.DeserializeObject<ImportSupplierDto[]>(inputJson);

            ICollection<Supplier> suppliers = new List<Supplier>();

            foreach (var dto in suppliersDtos)
            {
                Supplier supplier = Mapper.Map<Supplier>(dto);
                suppliers.Add(supplier);
            }

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}.";
        }

        //10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            ImportPartDto[] pDtos = JsonConvert.DeserializeObject<ImportPartDto[]>(inputJson)
                                        .Where(p => context.Suppliers.Any(s => s.Id == p.SupplierId))
                                        .ToArray();

            ICollection<Part> parts = new List<Part>();


            foreach (var pDto in pDtos)
            {
                Part part = Mapper.Map<Part>(pDto);
                parts.Add(part);

            }
            
            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}.";
        }

        //11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            ImportCarDto[] carDtos = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson)
                                        .Where(c => c.partsId.All(pId =>
                                                        context.Parts.Any(p => p.Id == pId)))
                                        .ToArray();

            ICollection<Car> cars = new List<Car>();

            foreach (var carDto in carDtos)
            {
                Car car = Mapper.Map<Car>(carDto);

                foreach (var id in carDto.partsId.Distinct())
                {
                    PartCar partCar = new PartCar()
                    {
                        PartId = id
                    };

                    car.PartCars.Add(partCar);
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }

        //12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            ImportCustomerDto[] customerDtos =
                JsonConvert.DeserializeObject<ImportCustomerDto[]>(inputJson);

            ICollection<Customer> customers = new List<Customer>();

            foreach (var cDto in customerDtos)
            {
                Customer customer = Mapper.Map<Customer>(cDto);
                customers.Add(customer);
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}.";
        }

        //13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            ImportSaleDto[] saleDtos = JsonConvert.DeserializeObject<ImportSaleDto[]>(inputJson);

            ICollection<Sale> sales = new List<Sale>();

            foreach (var sDto in saleDtos)
            {
                Sale sale = Mapper.Map<Sale>(sDto);
                sales.Add(sale);
            }

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }

        //14. Export Ordered Customers
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var orderedCustomers = context.Customers
                                    .OrderBy(c => c.BirthDate)
                                    .ThenBy(c => c.IsYoungDriver)
                                    .Select(c => new
                                    {
                                        Name = c.Name,
                                        BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                                        IsYoungDriver = c.IsYoungDriver
                                    })
                                    .ToArray();

            return JsonConvert.SerializeObject(orderedCustomers, Formatting.Indented);
        }

        //15. Export Cars From Make Toyota
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var toyotaCars = context.Cars
                                .Where(c => c.Make == "Toyota")
                                .OrderBy(c => c.Model)
                                .ThenByDescending(c => c.TravelledDistance)
                                .Select(c => new
                                {
                                    Id = c.Id,
                                    Make = c.Make,
                                    Model = c.Model,
                                    TravelledDistance = c.TravelledDistance
                                })
                                .ToArray();

            return JsonConvert.SerializeObject(toyotaCars, Formatting.Indented);
        }

        //16. Export Local Suppliers 
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                                .Where(s => !s.IsImporter)
                                .Select(s => new
                                {
                                    Id = s.Id,
                                    Name = s.Name,
                                    PartsCount = s.Parts.Count
                                })
                                .ToArray();

            return JsonConvert.SerializeObject(suppliers, Formatting.Indented);
        }

        //17. Export Cars With Their List Of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                            .Select(c => new
                            {
                                car = new
                                {
                                    Make = c.Make,
                                    Model = c.Model,
                                    TravelledDistance = c.TravelledDistance
                                },
                                parts = c.PartCars
                                            .Select(pc => new
                                            {
                                                Name = pc.Part.Name,
                                                Price = pc.Part.Price.ToString("F2")
                                            })
                                            .ToArray()
                            })
                            .ToArray();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

        //18. Export Total Sales By Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            //var customersOld = context.Customers
            //                    .Where(c => c.Sales.Any())
            //                    .Include(c => c.Sales)
            //                    .ThenInclude(s => s.Car)
            //                    .ThenInclude(c => c.PartCars)
            //                    .ThenInclude(pc => pc.Part)
            //                    .ToArray()
            //                    .Select(c => new
            //                    {
            //                        fullName = c.Name,
            //                        boughtCars = c.Sales.Count,
            //                        spentMoney = c.Sales.Sum(s =>
            //                                        s.Car.PartCars.Select(pc => pc.Part.Price).Sum())
            //                    })
            //                    .OrderByDescending(c => c.spentMoney)
            //                    .ThenByDescending(c => c.boughtCars)
            //                    .ToArray();

            var customers = context.Customers
                                .Where(c => c.Sales.Any())
                                .Select(c => new
                                {
                                    fullName = c.Name,
                                    boughtCars = c.Sales.Count,
                                    spentMoney = c.Sales.Sum(s => 
                                                    s.Car.PartCars.Sum(pc => pc.Part.Price))
                                })
                                .OrderByDescending(c => c.spentMoney)
                                .ThenByDescending(c => c.boughtCars)
                                .ToArray();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }

        //19. Export Sales With Applied Discount 
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                            .Take(10)
                            .Select(s => new
                            {
                                car = new
                                {
                                    Make = s.Car.Make,
                                    Model = s.Car.Model,
                                    TravelledDistance = s.Car.TravelledDistance
                                },
                                customerName = s.Customer.Name,
                                Discount = s.Discount.ToString("F2"),
                                price = s.Car.PartCars.Sum(pc => pc.Part.Price).ToString("F2"),
                                priceWithDiscount = (s.Car.PartCars.Sum(pc => pc.Part.Price)
                                                        * (1 - (s.Discount / 100))).ToString("F2")
                            })
                            .ToArray();

            return JsonConvert.SerializeObject(sales, Formatting.Indented);
        }

        static void CheckParts(CarDealerContext context, string json)
        {
            var parts = JsonConvert.DeserializeObject<ImportPartDto[]>(json)
                            .Where(p => !context.Suppliers.Any(s => s.Id == p.SupplierId))
                            .ToArray();

            foreach (var part in parts)
            {
                Console.WriteLine(part.Name + " - " + part.SupplierId);
            }
        }
    }
}