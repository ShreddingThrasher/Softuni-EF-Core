using CarDealer.Data;
using CarDealer.Dtos.Import;
using CarDealer.Models;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System.Collections.Generic;
using CarDealer.Dtos.Export;
using System.Text;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            CarDealerContext context = new CarDealerContext();

            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();

            //System.Console.WriteLine("Databasde reset successfull!");

            //string xml = File.ReadAllText("../../../Datasets/sales.xml");

            string result = GetSalesWithAppliedDiscount(context);

            System.Console.WriteLine(result);
        }

        //9. Import Suppliers
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Suppliers");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportSupplierDto[]), xmlRoot);

            using StringReader reader = new StringReader(inputXml);

            ImportSupplierDto[] sDtos = (ImportSupplierDto[])serializer.Deserialize(reader);

            Supplier[] suppliers = sDtos
                                    .Select(dto => new Supplier()
                                    {
                                        Name = dto.Name,
                                        IsImporter = dto.IsImporter
                                    })
                                    .ToArray();

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}";
        }


        //10. Import Parts
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Parts");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportPartDto[]), xmlRoot);

            using StringReader reader = new StringReader(inputXml);

            ImportPartDto[] pDtos = (ImportPartDto[])serializer.Deserialize(reader);

            Part[] parts = pDtos
                            .Where(p => context.Suppliers.Any(s => s.Id == p.SupplierId))
                            .Select(dto => new Part()
                            {
                                Name = dto.Name,
                                Price = dto.Price,
                                Quantity = dto.Quantity,
                                SupplierId = dto.SupplierId
                            })
                            .ToArray();

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Length}";
        }

        //11. Import Cars
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Cars");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportCarDto[]), xmlRoot);

            using StringReader reader = new StringReader(inputXml);

            ImportCarDto[] cDtos = (ImportCarDto[])serializer.Deserialize(reader);

            List<Car> cars = new List<Car>();

            foreach (var cDto in cDtos)
            {
                Car car = new Car();

                car.Make = cDto.Make;
                car.Model = cDto.Model;
                car.TravelledDistance = cDto.TraveledDistance;

                ICollection<PartCar> currCarParts = new List<PartCar>();

                foreach (int pId in cDto.Parts.Select(p => p.Id).Distinct())
                {
                    if(!context.Parts.Any(p => p.Id == pId))
                    {
                        continue;
                    }

                    currCarParts.Add(new PartCar()
                    {
                        Car = car,
                        PartId = pId
                    });
                }

                car.PartCars = currCarParts;
                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }


        //12. Import Customers
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Customers");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportCustomerDto[]), xmlRoot);

            using StringReader reader = new StringReader(inputXml);

            ImportCustomerDto[] cDtos = (ImportCustomerDto[])serializer.Deserialize(reader);

            Customer[] customers = cDtos
                                    .Select(dto => new Customer()
                                    {
                                        Name = dto.Name,
                                        BirthDate = dto.BirthDate,
                                        IsYoungDriver = dto.IsYoungDriver
                                    })
                                    .ToArray();

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Length}";
        }

        //13. Import Sales
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            XmlRootAttribute xmlRoot = new XmlRootAttribute("Sales");
            XmlSerializer serializer = new XmlSerializer(typeof(ImportSaleDto[]), xmlRoot);

            using StringReader reader = new StringReader(inputXml);

            ImportSaleDto[] sDtos = (ImportSaleDto[])serializer.Deserialize(reader);

            List<Sale> sales = new List<Sale>();

            foreach (var dto in sDtos)
            {
                if(!context.Cars.Any(c => c.Id == dto.CarId))
                {
                    continue;
                }

                Sale sale = new Sale();

                sale.CarId = dto.CarId;
                sale.CustomerId = dto.CustomerId;
                sale.Discount = dto.Discount;

                sales.Add(sale);
            }

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        //14. Export Cars With Distance
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            ExportCarsWithDistanceDto[] cDtos = context.Cars
                                                    .Where(c => c.TravelledDistance > 2000000)
                                                    .OrderBy(c => c.Make)
                                                    .ThenBy(c => c.Model)
                                                    .Take(10)
                                                    .Select(c => new ExportCarsWithDistanceDto()
                                                    {
                                                        Make = c.Make,
                                                        Model = c.Model,
                                                        TraveledDistance = c.TravelledDistance
                                                    })
                                                    .ToArray();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("cars");
            XmlSerializer serializer = new XmlSerializer(typeof(ExportCarsWithDistanceDto[]), xmlRoot);

            using StringWriter writer = new StringWriter(sb);

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            serializer.Serialize(writer, cDtos, namespaces);

            return sb.ToString().TrimEnd();
        }

        //15. Export Cars from make BMW
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            StringBuilder sb = new StringBuilder();

            ExportBMWCarsDto[] bmwCars = context
                .Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .Select(c => new ExportBMWCarsDto()
                {
                    Id = c.Id,
                    Model = c.Model,
                    TraveledDistance = c.TravelledDistance
                })
                .ToArray();

            XmlRootAttribute xmlRoot = new XmlRootAttribute("cars");
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ExportBMWCarsDto[]), xmlRoot);

            using StringWriter writer = new StringWriter(sb);

            xmlSerializer.Serialize(writer, bmwCars, namespaces);

            return sb.ToString().TrimEnd();
        }

        //16. Export Local Suppliers
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            ExportLocalSupplierDto[] dtos = context
                .Suppliers
                .Where(s => !s.IsImporter)
                .Select(s => new ExportLocalSupplierDto()
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();

            return Serialize<ExportLocalSupplierDto[]>("suppliers", dtos);
        }

        //17. Export Cars with Their List of Parts
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            ExportCarWithPartsDto[] dtos = context
                .Cars
                .Select(c => new ExportCarWithPartsDto()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TravelledDistance,
                    Parts = context.PartCars
                                .Where(pc => pc.CarId == c.Id)
                                .Select(pc => new ExportCarPartDto()
                                {
                                    Name = pc.Part.Name,
                                    Price = pc.Part.Price
                                })
                                .OrderByDescending(p => p.Price)
                                .ToArray()
                })
                .OrderByDescending(c => c.TraveledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ToArray();

            return Serialize<ExportCarWithPartsDto[]>("cars", dtos);
                
        }

        //18. Export Total Sales By Customer
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            ExportCustomerSalesDto[] dtos = context
                .Customers
                .Where(c => c.Sales.Any())
                .Select(c => new ExportCustomerSalesDto()
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales.Sum(s =>
                                    s.Car.PartCars.Sum(pc => pc.Part.Price))
                })
                .OrderByDescending(c => c.SpentMoney)
                .ToArray();

            return Serialize<ExportCustomerSalesDto[]>("customers", dtos);
        }

        //19. Export Sales with Applied Discount
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            ExportSaleDto[] dtos = context
                .Sales
                .Select(s => new ExportSaleDto()
                {
                    Car = new ExportSaleCarDto()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TravelledDistance
                    },
                    Discount = s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartCars.Sum(pc => pc.Part.Price),
                    PriceWithDiscount =
                        (s.Car.PartCars.Sum(pc => pc.Part.Price)) * (1 - (s.Discount / 100))
                })
                .ToArray();

            return Serialize<ExportSaleDto[]>("sales", dtos);
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