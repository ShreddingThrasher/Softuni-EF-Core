using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace CarDealer.DTO.Car
{
    [JsonObject]
    public class ImportCarDto
    {
        [JsonProperty("make")]
        public string Make { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("travelledDistance")]
        public long TravelledDistance { get; set; }

        [JsonProperty("partsId")]
        public List<int> partsId { get; set; }
    }
}
