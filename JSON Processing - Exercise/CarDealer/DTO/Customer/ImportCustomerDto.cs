using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CarDealer.DTO.Customer
{
    [JsonObject]
    public class ImportCustomerDto
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("birthdate")]
        public DateTime Birthdate { get; set; }

        [JsonProperty("isYoungDriver")]
        public bool IsYoungDriver { get; set; }
    }
}
