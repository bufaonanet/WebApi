using Microsoft.EntityFrameworkCore;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Data.Repo;

public class FurnishingTypeRepository : IFurnishingTypeRepository
{
    private readonly DataContext _context;

    public FurnishingTypeRepository(DataContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<FurnishingType>> GetFurnishingTypesAsync()
    {
        return await _context.FurnishingTypes.ToListAsync();
    }
}