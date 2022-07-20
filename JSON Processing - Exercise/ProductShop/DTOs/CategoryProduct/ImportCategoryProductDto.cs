using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ProductShop.DTOs.CategoryProduct
{
    [JsonObject]
    public class ImportCategoryProductDto
    {
        [JsonProperty("CategoryId")]
        public int CategoryId { get; set; }

        [JsonProperty("ProductId")]
        public int ProductId { get; set; }
    }
}
