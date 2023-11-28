using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class PropertyType : BaseEntity
{
    [Required] 
    public string Name { get; set; }
}