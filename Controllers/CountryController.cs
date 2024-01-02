// Controllers/CountryController.cs

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CountryApi.Data;
using CountryApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CountryApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CountryController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CountryController(AppDbContext context)
        {
            _context = context;
        }

        private static Expression<Func<Country, bool>> GetFilterExpression(string filter)
        {
            return c =>
                EF.Functions.Like(c.Name, $"%{filter}%") ||
                EF.Functions.Like(c.IsoCode, $"%{filter}%") ||
                c.Hotels.Any(h => EF.Functions.Like(h.Name, $"%{filter}%")) ||
                c.Restaurants.Any(r => EF.Functions.Like(r.Name, $"%{filter}%"));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Country>>> GetCountries(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int rowsPerPage = 10,
            [FromQuery] string filter = "")
        {
            var query = _context.Countries.Include(c => c.Hotels).Include(c => c.Restaurants).AsQueryable();

            if (!string.IsNullOrEmpty(filter))
            {
                query = query.Where(GetFilterExpression(filter));
            }

            int totalRecords = await query.CountAsync();

            var countries = await query.Skip((pageNumber - 1) * rowsPerPage)
                                       .Take(rowsPerPage)
                                       .ToListAsync();

            var countryResponses = countries.Select(c => new Country
            {
                Id = c.Id,
                Name = c.Name,
                IsoCode = c.IsoCode,
                Population = c.Population,
                Hotels = c.Hotels.Select(h => new Hotel
                {
                    Id = h.Id,
                    Name = h.Name,
                    Starts = h.Starts,
                    CountryId = c.Id,
                }).ToList(),
                Restaurants = c.Restaurants.Select(r => new Restaurant
                {
                    Id = r.Id,
                    Name = r.Name,
                    Type = r.Type,
                    CountryId = c.Id,
                }).ToList()
            }).ToList();

            Response.Headers.Add("X-Total-Records", totalRecords.ToString());

            return countryResponses;
        }
    }
}
