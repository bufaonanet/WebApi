using WebApi.Models;

namespace WebApi.Interfaces;

public interface ICityRepository
{
    Task<IEnumerable<City>> GetCitiesAsync();
    void AddCity(City city);
    void DeleteCity(int cityId);
    Task<City> FindCity(int id);
}