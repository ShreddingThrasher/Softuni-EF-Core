using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Artillery.DataProcessor.ImportDto
{
    [JsonObject]
    public class ImportGunCountriesDto
    {
        [JsonProperty("Id")]
        public int Id { get; set; }
    }
}
