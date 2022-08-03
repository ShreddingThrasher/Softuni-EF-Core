using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations;
using Theatre.Common;

namespace Theatre.DataProcessor.ImportDto
{
    [XmlType("Play")]
    public class ImportPlayDto
    {
        [Required]
        [MinLength(GlobalConstants.PlayTitleMinLength)]
        [MaxLength(GlobalConstants.PlayTitleMaxLength)]
        [XmlElement("Title")]
        public string Title { get; set; }

        [Required]
        [XmlElement("Duration")]
        public string Duration { get; set; }

        [Required]
        [Range(GlobalConstants.PlayRatingMinValue, GlobalConstants.PlayRatingMaxValue)]
        [XmlElement("Rating")]
        public float Rating { get; set; }

        [XmlElement("Genre")]
        public string Genre { get; set; }

        [XmlElement("Description")]
        public string Description { get; set; }

        [Required]
        [MinLength(GlobalConstants.PlayScreenwriterMinLength)]
        [MaxLength(GlobalConstants.PlayScreenwriterMaxLength)]
        [XmlElement("Screenwriter")]
        public string Screenwriter { get; set; }
    }
}
