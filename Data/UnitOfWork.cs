using WebApi.Data.Repo;
using WebApi.Interfaces;

namespace WebApi.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly DataContext _context;

    public UnitOfWork(DataContext context)
    {
        _context = context;
    }

    public ICityRepository CityRepository => new CityRepository(_context);
    
    public IUserRepository UserRepository => new UserRepository(_context);
   
    public IPropertyRepository PropertyRepository => new PropertyRepository(_context);
    
    public IFurnishingTypeRepository FurnishingTypeRepository => new FurnishingTypeRepository(_context);
    public async Task<bool> SaveAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}