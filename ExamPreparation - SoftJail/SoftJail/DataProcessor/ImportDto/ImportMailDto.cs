using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using SoftJail.Common;

namespace SoftJail.DataProcessor.ImportDto
{
    [JsonObject]
    public class ImportMailDto
    {
        [Required]
        [JsonProperty("Description")]
        public string Description { get; set; }

        [Required]
        [JsonProperty("Sender")]
        public string Sender { get; set; }

        [Required]
        [RegularExpression(GlobalConstants.MailAddressRegex)]
        [JsonProperty("Address")]
        public string Address { get; set; }
    }
}
