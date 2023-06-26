using System;

namespace EFCoreMovies.DTO.PostDTOs
{
    public class ActorCreationDTO
    {
        public string Name{ get; set; }
        public string Biography{ get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
