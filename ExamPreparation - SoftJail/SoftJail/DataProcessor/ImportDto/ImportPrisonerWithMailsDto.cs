using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using SoftJail.Common;

namespace SoftJail.DataProcessor.ImportDto
{
    [JsonObject]
    public class ImportPrisonerWithMailsDto
    {
        [Required]
        [MinLength(GlobalConstants.PrisonerFullNameMinLength)]
        [MaxLength(GlobalConstants.PrisonerFullNameMaxLength)]
        [JsonProperty("FullName")]
        public string FullName { get; set; }

        [Required]
        [RegularExpression(GlobalConstants.PrisonerNicknameRegex)]
        [JsonProperty("Nickname")]
        public string Nickname { get; set; }

        [Required]
        [Range(GlobalConstants.PrisonerAgeMinValue, GlobalConstants.PrisonerAgeMaxValue)]
        [JsonProperty("Age")]
        public int Age { get; set; }

        [Required]
        [JsonProperty("IncarcerationDate")]
        public string IncarcerationDate { get; set; }

        [JsonProperty("ReleaseDate")]
        public string ReleaseDate { get; set; }

        [Range(GlobalConstants.PrisonerBailMinValue, GlobalConstants.PrisonerBailMaxValue)]
        [JsonProperty("Bail")]
        public decimal? Bail { get; set; }

        [JsonProperty("CellId")]
        public int? CellId { get; set; }

        [JsonProperty("Mails")]
        public ImportMailDto[] Mails { get; set; }
    }
}
