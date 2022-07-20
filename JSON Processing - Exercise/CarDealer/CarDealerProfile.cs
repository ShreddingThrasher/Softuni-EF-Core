using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using CarDealer.DTO.Supplier;
using CarDealer.DTO.Part;
using CarDealer.Models;
using CarDealer.DTO.Car;
using CarDealer.DTO.Customer;
using CarDealer.DTO.Sale;
using System.Linq;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<ImportSupplierDto, Supplier>();
            this.CreateMap<ImportPartDto, Part>();
            this.CreateMap<ImportCarDto, Car>();
            this.CreateMap<ImportCustomerDto, Customer>();
            this.CreateMap<ImportSaleDto, Sale>();
        }
    }
}
