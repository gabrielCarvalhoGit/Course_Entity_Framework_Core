using System.ComponentModel.DataAnnotations;

namespace EFCoreMovies.DTO.PostDTOs
{
    public class GenreCreationDTO
    {
        [Required]
        public string Name { get; set; }
    }
}
