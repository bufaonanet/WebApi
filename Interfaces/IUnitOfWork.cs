namespace WebApi.Interfaces;

public interface IUnitOfWork
{
    ICityRepository CityRepository {get; }

    IUserRepository UserRepository {get; }
    
    IPropertyRepository PropertyRepository {get; }
    
    IFurnishingTypeRepository FurnishingTypeRepository {get; }

    Task<bool> SaveAsync();
}