// Controllers/ExternalCountryController.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CountryApi.Data;
using CountryApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Bogus;
using Bogus.DataSets;

namespace CountryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalCountryController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly AppDbContext _context;

        public ExternalCountryController(HttpClient httpClient, AppDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        [HttpPost("GenerateHotelsAndRestaurants")]
        public async Task<IActionResult> GenerateHotelsAndRestaurants()
        {
            // Obtener todos los países de la base de datos
            var allCountries = await _context.Countries.ToListAsync();

            // Mezclar aleatoriamente los países
            var randomCountries = allCountries.OrderBy(x => Guid.NewGuid()).Take(5).ToList();

            // Generar 2 hoteles y 2 restaurantes asignados aleatoriamente a estos cinco países
            var hotels = new List<Hotel>();
            var restaurants = new List<Restaurant>();

            var faker = new Faker();

            foreach (var country in randomCountries)
            {
                var hotelsForCountry = Enumerable.Range(0, 2)
                    .Select(_ => new Hotel { Name = faker.Company.CompanyName(), Starts = faker.Random.Number(1, 5), Country = country })
                    .ToList();
                
                var restaurantsForCountry = Enumerable.Range(0, 2)
                    .Select(_ => new Restaurant { Name = faker.Company.CompanyName(), Type = faker.Random.Word(), Country = country })
                    .ToList();

                hotels.AddRange(hotelsForCountry);
                restaurants.AddRange(restaurantsForCountry);
            }

            // Agregar hoteles y restaurantes a la base de datos
            await _context.Hotels.AddRangeAsync(hotels);
            await _context.Restaurants.AddRangeAsync(restaurants);

            // Guardar cambios en la base de datos
            await _context.SaveChangesAsync();

            return Ok("2 hotels and 2 restaurants generated and assigned to 5 random countries successfully");
        }


        [HttpPost("FillCountryFromExternalApi")]
        public async Task<IActionResult> FillCountryFromExternalApi()
        {
            var response = await _httpClient.GetAsync("https://restcountries.com/v3.1/all");

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();

                // Utilizar JArray para manejar un array JSON
                JArray jsonArray = JArray.Parse(jsonString);

                int recordsProcessed = 0;

                foreach (var country in jsonArray)
                {
                    // Convertir el objeto JSON a un objeto fuertemente tipado
                    var countryObject = country.ToObject<CountryApiResponse>();

                    var commonName = countryObject.Name?.Common;
                    var isoCode = countryObject.Cca2;
                    var population = countryObject.Population;

                    // Comprobar si el país ya existe en la base de datos
                    var existingCountry = await _context.Countries.FirstOrDefaultAsync(c => c.IsoCode == isoCode);

                    if (existingCountry == null)
                    {
                        // Si no existe, agregarlo a la base de datos
                        var newCountry = new Country { Name = commonName, IsoCode = isoCode, Population = population ?? 0 };
                        _context.Countries.Add(newCountry);

                        await _context.SaveChangesAsync(); // Guardar el país en la base de datos antes de continuar
                        recordsProcessed++;
                    }
                    else
                    {
                        // Si existe, actualizar propiedades específicas si es necesario
                        existingCountry.Name = commonName;
                        existingCountry.Population = population ?? 0;

                        recordsProcessed++;
                    }
                }

                Console.WriteLine("Received data from external API:");
                return Ok($"{recordsProcessed} countries processed successfully");
            }
            else
            {
                return StatusCode((int)response.StatusCode, "Error al obtener datos de la API externa");
            }
        }
    }
}
