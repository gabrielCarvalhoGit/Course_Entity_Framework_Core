namespace EFCoreMovies.DTO
{
    public class MovieFilterDTO
    {
        public string Title { get; set; }
        public int GenreId { get; set; }
        public bool inCinemas { get; set; }
        public bool UpcomingReleases { get; set; }
    }
}
