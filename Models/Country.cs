using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace CountryApi.Models
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; 
        public string IsoCode { get; set; } = string.Empty; 
        public List<Hotel>? Hotels { get; set; }
        public List<Restaurant>? Restaurants { get; set; }
    }
}
