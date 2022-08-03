using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace TeisterMask.DataProcessor.ExportDto
{
    [JsonObject]
    public class ExportEmployeeTaskDto
    {
        [JsonProperty("TaskName")]
        public string TaskName { get; set; }

        [JsonProperty("OpenDate")]
        public string OpenDate { get; set; }

        [JsonProperty("DueDate")]
        public string DueDate { get; set; }

        [JsonProperty("LabelType")]
        public string LabelType { get; set; }

        [JsonProperty("ExecutionType")]
        public string ExecutionType { get; set; }
    }
}
