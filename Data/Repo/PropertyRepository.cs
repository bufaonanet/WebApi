using Microsoft.EntityFrameworkCore;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Data.Repo;

public class PropertyRepository : IPropertyRepository
{
    private readonly DataContext _context;

    public PropertyRepository(DataContext context)
    {
        _context = context;
    }

    public void AddProperty(Property property)
    {
        _context.Properties.Add(property);
    }

    public void DeleteProperty(int id)
    {
        throw new System.NotImplementedException();
    }

    public async Task<IEnumerable<Property>> GetPropertiesAsync(int sellRent)
    {
        var properties = await _context.Properties
            .Include(p => p.PropertyType)
            .Include(p => p.City)
            .Include(p => p.FurnishingType)
            .Include(p => p.Photos)
            .Where(p => p.SellRent == sellRent)
            .ToListAsync();

        return properties;
    }

    public async Task<Property> GetPropertyDetailAsync(int id)
    {
        var properties = await _context.Properties
            .Include(p => p.PropertyType)
            .Include(p => p.City)
            .Include(p => p.FurnishingType)
            .Include(p => p.Photos)
            .Where(p => p.Id == id)
            .FirstAsync();

        return properties;
    }

    public async Task<Property> GetPropertyByIdAsync(int id)
    {
        var properties = await _context.Properties
            .Include(p => p.Photos)
            .Where(p => p.Id == id)
            .FirstOrDefaultAsync();

        return properties;
    }
}