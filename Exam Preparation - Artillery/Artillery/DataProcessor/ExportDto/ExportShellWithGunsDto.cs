using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Artillery.DataProcessor.ExportDto
{
    [JsonObject]
    public class ExportShellWithGunsDto
    {
        [JsonProperty("ShellWeight")]
        public double ShellWeight { get; set; }

        [JsonProperty("Caliber")]
        public string Caliber { get; set; }

        [JsonProperty("Guns")]
        public ExportShellGunDto[] Guns { get; set; }
    }
}
