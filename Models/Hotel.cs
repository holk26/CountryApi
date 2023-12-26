using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// CountryApi/Models/Hotel.cs
namespace CountryApi.Models
{
    public class Hotel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; 
        public int CountryId { get; set; }
        public Country Country { get; set; } = new Country();
    }
}
