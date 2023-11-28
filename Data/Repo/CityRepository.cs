using Microsoft.EntityFrameworkCore;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Data.Repo;

public class CityRepository : ICityRepository
{
    private readonly DataContext _context;

    public CityRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<City>> GetCitiesAsync()
    {
        return await _context.Cities.ToListAsync();
    }

    public void AddCity(City city)
    {
        _context.Cities.Add(city);
    }

    public async void DeleteCity(int cityId)
    {
        var city = await _context.Cities.FindAsync(cityId);
        if (city != null)
            _context.Cities.Remove(city);
    }

    public async Task<City> FindCity(int id)
    {
        return await _context.Cities.FindAsync(id);
    }

    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}