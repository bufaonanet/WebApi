using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Controllers;
using WebApi.DTOs;
using WebApi.Interfaces;

namespace WebApi;

public class FurnishingTypeController : BaseController
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public FurnishingTypeController(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }
    
    // GET api/furnishingtypes
    [HttpGet ("list")]        
    [AllowAnonymous]
    public async Task<IActionResult> GetFurnishingTypes()
    {            
        var furnishingTypes = await _uow.FurnishingTypeRepository.GetFurnishingTypesAsync();
        var furnishingTypesDto = _mapper.Map<IEnumerable<KeyValuePairDto>>(furnishingTypes);
        return Ok(furnishingTypesDto);
    }
}