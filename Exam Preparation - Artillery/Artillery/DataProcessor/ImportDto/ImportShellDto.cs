using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Artillery.Common;

namespace Artillery.DataProcessor.ImportDto
{
    [XmlType("Shell")]
    public class ImportShellDto
    {
        [Required]
        [Range(GlobalConstants.ShellWeightMinValue, GlobalConstants.ShellWeightMaxValue)]
        [XmlElement("ShellWeight")]
        public double ShellWeight { get; set; }

        [Required]
        [MinLength(GlobalConstants.ShellCaliberMinLength)]
        [MaxLength(GlobalConstants.ShellCaliberMaxLength)]
        [XmlElement("Caliber")]
        public string Caliber { get; set; }
    }
}
