using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SoftJail.DataProcessor.ExportDto
{
    public class ExportPrisonerWithCellsAndOfficers
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("CellNumber")]
        public int CellNumber { get; set; }

        [JsonProperty("Officers")]
        public ExportPrisonerOfficerDto[] Officers { get; set; }

        [JsonProperty("TotalOfficerSalary")]
        public decimal TotalOfficerSalary { get; set; }
    }
}
