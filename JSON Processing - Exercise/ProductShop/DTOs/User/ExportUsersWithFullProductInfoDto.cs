using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using ProductShop.DTOs.Product;

namespace ProductShop.DTOs.User
{
    [JsonObject]
    public class ExportUsersWithFullProductInfoDto
    {
        [JsonProperty("lastName")]
        public string LastName { get; set; }

        [JsonProperty("age")]
        public int? Age { get; set; }

        [JsonProperty("soldProducts")]
        public ExportSoldProductsFullInfoDto SoldProductsInfo { get; set; }
    }
}
