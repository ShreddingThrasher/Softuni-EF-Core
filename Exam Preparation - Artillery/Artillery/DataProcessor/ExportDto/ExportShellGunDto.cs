using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Artillery.DataProcessor.ExportDto
{
    public class ExportShellGunDto
    {
        [JsonProperty("GunType")]
        public string GunType { get; set; }

        [JsonProperty("GunWeight")]
        public int GunWeight { get; set; }

        [JsonProperty("BarrelLength")]
        public double BarrelLength { get; set; }

        [JsonProperty("Range")]
        public string Range { get; set; }
    }
}
