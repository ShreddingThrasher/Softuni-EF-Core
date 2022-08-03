using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;
using TeisterMask.Common;

namespace TeisterMask.DataProcessor.ImportDto
{
    [JsonObject]
    public class ImportEmployeeDto
    {
        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        [RegularExpression(GlobalConstants.EmployeeUsernameRegex)]
        [JsonProperty("Username")]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [JsonProperty("Email")]
        public string Email { get; set; }

        [Required]
        [MaxLength(GlobalConstants.EmployeePhoneMaxLength)]
        [RegularExpression(GlobalConstants.EmployeePhoneRegex)]
        [JsonProperty("Phone")]
        public string Phone { get; set; }

        [JsonProperty("Tasks")]
        public int[] Tasks { get; set; }
    }
}
