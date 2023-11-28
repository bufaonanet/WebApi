using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.Interfaces;
using WebApi.Models;

namespace WebApi.Controllers;

public class PropertyController : BaseController
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;
    private readonly IPhotoService _photoService;

    public PropertyController(IUnitOfWork uow, IMapper mapper, IPhotoService photoService)
    {
        _uow = uow;
        _mapper = mapper;
        _photoService = photoService;
    }
    
    //property/list/1
    [HttpGet("list/{sellRent}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPropertyList(int sellRent)
    {
        var properties = await _uow.PropertyRepository.GetPropertiesAsync(sellRent);
        var propertyListDto = _mapper.Map<IEnumerable<PropertyListDto>>(properties);
        return Ok(propertyListDto);
    }
    
    //property/detail/1
    [HttpGet("detail/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPropertyDetail(int id)
    {
        var property = await _uow.PropertyRepository.GetPropertyDetailAsync(id);
        var propertyDto = _mapper.Map<PropertyDetailDto>(property);
        return Ok(propertyDto);
    }
    
    //property/add
    [HttpPost("add")]
    [Authorize]
    public async Task<IActionResult> AddProperty(PropertyDto propertyDto)
    {
        var property = _mapper.Map<Property>(propertyDto);
        var userId = GetUserId();
        property.PostedBy = userId;
        property.LastUpdatedBy = userId;
        _uow.PropertyRepository.AddProperty(property);
        await _uow.SaveAsync();
        return StatusCode(201);
    }
    
    //property/add/photo/1
        [HttpPost("add/photo/{propId}")]
        [Authorize]
        public async Task<ActionResult<PhotoDto>> AddPropertyPhoto(IFormFile file, int propId)
        {
            var result = await _photoService.UploadPhotoAsync(file);
            if (result.Error != null)
                return BadRequest(result.Error.Message);
            var userId = GetUserId();

            var property = await _uow.PropertyRepository.GetPropertyByIdAsync(propId);

            if (property.PostedBy != userId)
                return BadRequest("You are not authorised to upload photo for this property");

            var photo = new Photo
            {
                ImageUrl = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (property.Photos.Count == 0)
            {
                photo.IsPrimary = true;
            }

            property.Photos.Add(photo);
            
            if (await _uow.SaveAsync()) 
                return _mapper.Map<PhotoDto>(photo);

            return BadRequest("Some problem occured in uploading photo..");
        }

        //property/set-primary-photo/42/jl0sdfl20sdf2s
        [HttpPost("set-primary-photo/{propId}/{photoPublicId}")]
        [Authorize]
        public async Task<IActionResult> SetPrimaryPhoto(int propId, string photoPublicId)
        {
            var userId = GetUserId();

            var property = await _uow.PropertyRepository.GetPropertyByIdAsync(propId);

            if (property.PostedBy != userId)
                return BadRequest("You are not authorised to change the photo");

            if (property == null || property.PostedBy != userId)
                return BadRequest("No such property or photo exists");

            var photo = property.Photos.FirstOrDefault(p => p.PublicId == photoPublicId);

            if (photo == null)
                return BadRequest("No such property or photo exists");

            if (photo.IsPrimary)
                return BadRequest("This is already a primary photo");


            var currentPrimary = property.Photos.FirstOrDefault(p => p.IsPrimary);
            if (currentPrimary != null) currentPrimary.IsPrimary = false;
            photo.IsPrimary = true;

            if (await _uow.SaveAsync()) return NoContent();

            return BadRequest("Failed to set primary photo");
        }

        [HttpDelete("delete-photo/{propId}/{photoPublicId}")]
        [Authorize]
        public async Task<IActionResult> DeletePhoto(int propId, string photoPublicId)
        {
            var userId = GetUserId();

            var property = await _uow.PropertyRepository.GetPropertyByIdAsync(propId);

            if (property.PostedBy != userId)
                return BadRequest("You are not authorised to delete the photo");

            if (property == null || property.PostedBy != userId)
                return BadRequest("No such property or photo exists");

            var photo = property.Photos.FirstOrDefault(p => p.PublicId == photoPublicId);

            if (photo == null)
                return BadRequest("No such property or photo exists");

            if (photo.IsPrimary)
                return BadRequest("You can not delete primary photo");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            property.Photos.Remove(photo);

            if (await _uow.SaveAsync()) return Ok();

            return BadRequest("Failed to delete photo");
        }
}