using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Artillery.Common;

namespace Artillery.DataProcessor.ImportDto
{
    [JsonObject]
    public class ImportGunDto
    {
        [Required]
        [JsonProperty("ManufacturerId")]
        public int ManufacturerId { get; set; }

        [Required]
        [Range(GlobalConstants.GunWeightMinValue, GlobalConstants.GunWeightMaxValue)]
        [JsonProperty("GunWeight")]
        public int GunWeight { get; set; }

        [Required]
        [Range(GlobalConstants.GunBarrelLengthMinValue, GlobalConstants.GunBarrelLengthMaxValue)]
        [JsonProperty("BarrelLength")]
        public double BarrelLength { get; set; }

        [JsonProperty("NumberBuild")]
        public int? NumberBuild { get; set; }

        [Required]
        [Range(GlobalConstants.GunRangeMinValue, GlobalConstants.GunRangeMaxValue)]
        [JsonProperty("Range")]
        public int Range { get; set; }


        [Required]
        [JsonProperty("GunType")]
        public string GunType { get; set; }

        [Required]
        [JsonProperty("ShellId")]
        public int ShellId { get; set; }

        [JsonProperty("Countries")]
        public ImportGunCountriesDto[] Countries { get; set; }
    }
}
