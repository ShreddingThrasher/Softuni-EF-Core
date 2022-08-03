using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Theatre.DataProcessor.ExportDto
{
    [JsonObject]
    public class ExportTopTheaterDto
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Halls")]
        public int Halls { get; set; }

        [JsonProperty("TotalIncome")]
        public decimal TotalIncome { get; set; }

        [JsonProperty("Tickets")]
        public ExportTheaterTicketDto[] Tickets { get; set; }
    }
}
