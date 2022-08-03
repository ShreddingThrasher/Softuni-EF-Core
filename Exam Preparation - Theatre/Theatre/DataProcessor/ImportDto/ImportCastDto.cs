using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using Theatre.Common;

namespace Theatre.DataProcessor.ImportDto
{
    [XmlType("Cast")]
    public class ImportCastDto
    {
        [Required]
        [MinLength(GlobalConstants.CastFullNameMinLength)]
        [MaxLength(GlobalConstants.CastFullNameMaxLength)]
        public string FullName { get; set; }

        [Required]
        public bool IsMainCharacter { get; set; }

        [Required]
        [MaxLength(GlobalConstants.CastPhoneNumberMaxLength)]
        [RegularExpression(GlobalConstants.CastPhoneNumberRegex)]
        public string PhoneNumber { get; set; }

        [Required]
        public int PlayId { get; set; }
    }
}
