using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class FurnishingType : BaseEntity
{
    [Required] 
    public string Name { get; set; }
}