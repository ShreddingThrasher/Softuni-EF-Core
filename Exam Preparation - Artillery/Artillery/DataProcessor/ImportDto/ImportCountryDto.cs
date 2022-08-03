using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Artillery.Common;

namespace Artillery.DataProcessor.ImportDto
{
    [XmlType("Country")]
    public class ImportCountryDto
    {
        [Required]
        [MinLength(GlobalConstants.CountryNameMinLength)]
        [MaxLength(GlobalConstants.CountryNameMaxLength)]
        [XmlElement("CountryName")]
        public string CountryName { get; set; }

        [Required]
        [Range(GlobalConstants.CountryArmySizeMinValue, GlobalConstants.CountryArmySizeMaxValue)]
        [XmlElement("ArmySize")]
        public int ArmySize { get; set; }
    }
}
