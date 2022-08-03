using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Artillery.Common;

namespace Artillery.DataProcessor.ImportDto
{
    [XmlType("Manufacturer")]
    public class ImportManufacturerDto
    {
        [Required]
        [MinLength(GlobalConstants.ManufacturerNameMinLength)]
        [MaxLength(GlobalConstants.ManufacturerNameMaxLength)]
        [XmlElement("ManufacturerName")]
        public string ManufacturerName { get; set; }

        [Required]
        [MinLength(GlobalConstants.ManufacturerFoundedMinLength)]
        [MaxLength(GlobalConstants.ManufacturerFoundedMaxLength)]
        [XmlElement("Founded")]
        public string Founded { get; set; }
    }
}
