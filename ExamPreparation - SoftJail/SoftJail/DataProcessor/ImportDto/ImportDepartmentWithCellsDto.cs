using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using SoftJail.Common;

namespace SoftJail.DataProcessor.ImportDto
{
    [JsonObject]
    public class ImportDepartmentWithCellsDto
    {
        [Required]
        [MinLength(GlobalConstants.DepartmentNameMinLength)]
        [MaxLength(GlobalConstants.DepartmentNameMaxLength)]
        [JsonProperty("Name")]
        public string Name { get; set; }

        [Required]
        [JsonProperty("Cells")]
        public ImportCellDto[] Cells { get; set; }
    }
}
