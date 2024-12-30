using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos;

public record class UpdateGameDto
(
    [Required][StringLength(50)]string Name, 
    [Required][StringLength(50)]string Genre,
    [Range(1,5000)]decimal Price,
    DateOnly ReleaseDate);
