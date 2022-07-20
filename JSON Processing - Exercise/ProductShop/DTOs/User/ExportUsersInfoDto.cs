using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace ProductShop.DTOs.User
{
    [JsonObject]
    public class ExportUsersInfoDto
    {
        [JsonProperty("usersCount")]
        public int UsersCount { get; set; }

        [JsonProperty("users")]
        public ExportUsersWithFullProductInfoDto[] Users { get; set; }
    }
}
