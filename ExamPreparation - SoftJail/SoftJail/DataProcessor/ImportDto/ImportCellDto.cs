using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;
using SoftJail.Common;

namespace SoftJail.DataProcessor.ImportDto
{
    [JsonObject]
    public class ImportCellDto
    {
        [Required]
        [Range(GlobalConstants.CellNumberMinValue, GlobalConstants.CellNumberMaxValue)]
        [JsonProperty("CellNumber")]
        public int CellNumber { get; set; }

        [Required]
        [JsonProperty("HasWindow")]
        public bool HasWindow { get; set; }
    }
}
