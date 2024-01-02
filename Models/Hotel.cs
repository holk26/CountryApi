using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace CountryApi.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Starts { get; set; }
        public int CountryId { get; set; }

        [JsonIgnore]
        public Country Country { get; set; }
    }
}
