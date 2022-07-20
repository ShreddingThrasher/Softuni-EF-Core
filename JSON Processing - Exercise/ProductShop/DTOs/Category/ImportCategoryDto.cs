using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ProductShop.DTOs.Category
{
    [JsonObject]
    public class ImportCategoryDto
    {
        [JsonProperty("name")]
        [Required]
        [MinLength(GlobalConstants.CategoryNameMinLength)]
        [MaxLength(GlobalConstants.CategoryNameMaxLength)]
        public string Name { get; set; }
    }
}
