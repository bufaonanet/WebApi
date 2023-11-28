using WebApi.Models;

namespace WebApi.Interfaces;

public interface IPropertyTypeRepository
{
    Task<IEnumerable<PropertyType>> GetPropertyTypesAsync();         
}