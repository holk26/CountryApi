using Newtonsoft.Json;

namespace CountryApi.Models
{
    public class CountryApiResponse
    {
        [JsonProperty("name")]
        public Name Name { get; set; }

        [JsonProperty("cca2")]
        public string Cca2 { get; set; }

        [JsonProperty("population")]
        public int? Population { get; set; }
    }

    public class Name
    {
        [JsonProperty("common")]
        public string Common { get; set; }
    }
}
