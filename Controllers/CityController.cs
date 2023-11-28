using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Controllers;


[Authorize]
public class CityController : BaseController
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CityController(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    // GET api/city
    [HttpGet("cities")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCities()
    {
        var cities = await _uow.CityRepository.GetCitiesAsync();

        var citiesDto = _mapper.Map<IEnumerable<CityDto>>(cities);

        return Ok(citiesDto);
    }

    [HttpPost("post")]
    public async Task<IActionResult> AddCity(CityDto cityDto)
    {
        var city = _mapper.Map<City>(cityDto);
        city.LastUpdatedBy = 1;
        city.LastUpdatedOn = DateTime.Now;

        _uow.CityRepository.AddCity(city);
        await _uow.SaveAsync();

        return StatusCode(201);
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateCity(int id, CityDto cityDto)
    {
        if (id != cityDto.Id)
            return BadRequest("Update not allowed");

        var cityFromDb = await _uow.CityRepository.FindCity(id);

        if (cityFromDb == null)
            return BadRequest("Update not allowed");

        cityFromDb.LastUpdatedBy = 1;
        cityFromDb.LastUpdatedOn = DateTime.Now;
        _mapper.Map(cityDto, cityFromDb);

        await _uow.SaveAsync();
        return StatusCode(200);
    }

    [HttpPut("updateCityName/{id:int}")]
    public async Task<IActionResult> UpdateCity(int id, CityUpdateDto cityDto)
    {
        var cityFromDb = await _uow.CityRepository.FindCity(id);
        cityFromDb.LastUpdatedBy = 1;
        cityFromDb.LastUpdatedOn = DateTime.Now;
        _mapper.Map(cityDto, cityFromDb);
        await _uow.SaveAsync();
        return StatusCode(200);
    }

    [HttpPatch("update/{id:int}")]
    public async Task<IActionResult> UpdateCityPatch(int id, JsonPatchDocument<City> cityToPatch)
    {
        var cityFromDb = await _uow.CityRepository.FindCity(id);
        cityFromDb.LastUpdatedBy = 1;
        cityFromDb.LastUpdatedOn = DateTime.Now;

        cityToPatch.ApplyTo(cityFromDb, ModelState);
        await _uow.SaveAsync();
        return StatusCode(200);
    }

    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> AddCity(int id)
    {
        var city = await _uow.CityRepository.FindCity(id);
        if (city is null) return NotFound();

        _uow.CityRepository.DeleteCity(id);
        await _uow.SaveAsync();

        return Ok(id);
    }
}