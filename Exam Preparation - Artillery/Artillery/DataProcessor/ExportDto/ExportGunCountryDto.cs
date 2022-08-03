using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Artillery.DataProcessor.ExportDto
{
    [XmlType("Country")]
    public class ExportGunCountryDto
    {
        [XmlAttribute("Country")]
        public string Name { get; set; }

        [XmlAttribute("ArmySize")]
        public int ArmySize { get; set; }
    }
}
