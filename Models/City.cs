using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class City : BaseEntity
{
    [Column(Order = 1)]
    public string Name { get; set; }

    [Required]
    [Column(Order = 2)]
    public string Country { get; set; }
}