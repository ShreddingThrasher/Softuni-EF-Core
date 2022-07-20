using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ProductShop.DTOs.Product
{
    [JsonObject]
    public class ImportProductDto
    {
        [JsonProperty("Name")]
        [Required]
        [MinLength(GlobalConstants.ProductNameMinLength)]
        public string Name { get; set; }

        [JsonProperty("Price")]
        public decimal Price { get; set; }

        [JsonProperty("BuyerId")]
        public int? BuyerId { get; set; }

        [JsonProperty("SellerId")]
        public int SellerId { get; set; }
    }
}
