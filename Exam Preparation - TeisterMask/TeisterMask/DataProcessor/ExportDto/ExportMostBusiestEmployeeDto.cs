using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TeisterMask.DataProcessor.ExportDto
{
    [JsonObject]
    public class ExportMostBusiestEmployeeDto
    {
        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("Tasks")]
        public ExportEmployeeTaskDto[] Tasks { get; set; }
    }
}
