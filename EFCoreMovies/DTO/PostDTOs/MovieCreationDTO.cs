using System;
using System.Collections.Generic;

namespace EFCoreMovies.DTO.PostDTOs
{
    public class MovieCreationDTO
    {
        public string Title { get; set; }
        public bool InCinemas { get; set; }
        public DateTime ReleaseDate { get; set; }
        public List<int> GenresIds { get; set; }
        public List<int> CinemaHallsIds { get; set; }
        public List<MovieActorCreationDTO> MovieActors { get; set; }
    }
}
