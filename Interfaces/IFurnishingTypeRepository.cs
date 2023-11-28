using WebApi.Models;

namespace WebApi.Interfaces;

public interface IFurnishingTypeRepository
{
    Task<IEnumerable<FurnishingType>> GetFurnishingTypesAsync();
}