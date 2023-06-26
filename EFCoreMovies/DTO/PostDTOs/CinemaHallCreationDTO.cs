using EFCoreMovies.Entities;

namespace EFCoreMovies.DTO.PostDTOs
{
    public class CinemaHallCreationDTO
    {
        public int Id { get; set; }
        public double Cost { get; set; }
        public CinemaHallType CinemaHalltype { get; set; }
    }
}
