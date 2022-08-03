using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using Theatre.Common;

namespace Theatre.DataProcessor.ImportDto
{
    [JsonObject]
    public class ImportTicketDto
    {
        [Required]
        [Range(GlobalConstants.TicketPriceMinValue, GlobalConstants.TicketPriceMaxValue)]
        [JsonProperty("Price")]
        public decimal Price { get; set; }

        [Required]
        [Range(GlobalConstants.TicketRowNumberMinValue, GlobalConstants.TicketRowNumberMaxValue)]
        [JsonProperty("RowNumber")]
        public sbyte RowNumber { get; set; }

        [Required]
        [JsonProperty("PlayId")]
        public int PlayId { get; set; }
    }
}
