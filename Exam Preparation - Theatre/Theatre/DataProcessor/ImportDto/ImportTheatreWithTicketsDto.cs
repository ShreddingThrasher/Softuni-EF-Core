using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Theatre.Common;

namespace Theatre.DataProcessor.ImportDto
{
    [JsonObject]
    public class ImportTheatreWithTicketsDto
    {
        [Required]
        [MinLength(GlobalConstants.TheatreNameMinLength)]
        [MaxLength(GlobalConstants.TheatreNameMaxLength)]
        [JsonProperty("Name")]
        public string Name { get; set; }

        [Required]
        [Range(GlobalConstants.TheatreHallsMinValue, GlobalConstants.TheatreHallsMaxValue)]
        [JsonProperty("NumberOfHalls")]
        public sbyte NumberOfHalls { get; set; }

        [Required]
        [MinLength(GlobalConstants.TheatreDirectorMinLength)]
        [MaxLength(GlobalConstants.TheatreDirectorMaxLength)]
        [JsonProperty("Director")]
        public string Director { get; set; }

        [JsonProperty("Tickets")]
        public ImportTicketDto[] Tickets { get; set; }
    }
}
