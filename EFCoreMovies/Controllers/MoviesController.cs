using AutoMapper;
using AutoMapper.QueryableExtensions;
using EFCoreMovies.Date;
using EFCoreMovies.DTO;
using EFCoreMovies.DTO.PostDTOs;
using EFCoreMovies.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EFCoreMovies.Controllers
{
    [ApiController]
    [Route("api/Movies")]
    public class MoviesController : ControllerBase
    {
        private readonly EFCoreDbContext _dbContext;
        private readonly IMapper _mapper;
        public MoviesController(EFCoreDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MovieDTO>> Get(int id)
        {
            var movie = await _dbContext.Movies
                .Include(m => m.Genres.OrderByDescending(g => g.Id).Where(g => !g.Name.Contains("m"))) //OrderByDescending organiza os dados de maneira decrescente
                .Include(m => m.CinemaHalls.OrderByDescending(ch => ch.Cinema.Name)) //Include Traz os dados relacionados a tabela de genêros
                    .ThenInclude(ch => ch.Cinema) //TheInclude Traz os dados relacionados a tabela CinemaHall mas que não estão ligados diretamente a tabela principal
                .Include(m => m.MovieActors)
                    .ThenInclude(m => m.Actor)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie is null)
            {
                return NotFound();
            }

            var movieDTO = _mapper.Map<MovieDTO>(movie);

            movieDTO.Cinemas = movieDTO.Cinemas.DistinctBy(m => m.Id).ToList();

            return movieDTO;
        }

        [HttpGet("AutoMapper/{id:int}")]
        public async Task<ActionResult<MovieDTO>> GetWithAutoMapper(int id)
        {
            var movieDTO = await _dbContext.Movies
                    .ProjectTo<MovieDTO>(_mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(m => m.Id == id);

            if (movieDTO is null)
            {
                return NotFound();
            }

            movieDTO.Cinemas = movieDTO.Cinemas.DistinctBy(x => x.Id == id).ToList();

            return movieDTO;
        }

        /* Outros Métodos
        [HttpGet("ExplicitLoading/{id:int}")]
        public async Task<ActionResult<MovieDTO>> GetExplicitLoading(int id)
        {
            var movie = await _dbContext.Movies.FirstOrDefaultAsync(m => m.Id == id);

            if (movie is null)
            {
                return NotFound();
            }

            var genresCount = await _dbContext.Entry(movie).Collection(p => p.Genres).Query().CountAsync();

            var movieDTO = _mapper.Map<MovieDTO>(movie);

            return Ok(new
            {
                Id = movieDTO.Id,
                Title = movieDTO.Title,
                GenresCount = genresCount
            });
        }

        [HttpGet("LazyLoading/{id:int}")]
        public async Task<ActionResult<MovieDTO>> GetLazyLoading(int id)
        {
            var movie = await _dbContext.Movies.FirstOrDefaultAsync(p => p.Id == id);

            var movieDTO = _mapper.Map<MovieDTO>(movie);

            movieDTO.Cinemas = movieDTO.Cinemas.DistinctBy(x => x.Id).ToList();

            return movieDTO;
        }

        [HttpGet("GroupByCinemas")]
        public async Task<ActionResult> GetGroupByCinemas()
        {
            var groupedMovies = await _dbContext.Movies.GroupBy(m => m.InCinemas).Select(g => new
            {
                InCinemas = g.Key,
                Count = g.Count(),
                Movies = g.ToList()
            }).ToListAsync();

            return Ok(groupedMovies);
        }


        [HttpGet("GroupByGenresCount")]
        public async Task<ActionResult> GetGroupByGenresCount()
        {
            var groupedMovies = await _dbContext.Movies.GroupBy(m => m.Genres.Count()).Select(g => new
            {
                Count = g.Key,
                Titles = g.Select(x => x.Title),
                Genres = g.Select(m => m.Genres).SelectMany(a => a).Select(ge => ge.Name).Distinct()
            }).ToListAsync();

            return Ok(groupedMovies);
        }

        [HttpGet("Filter")]
        public async Task<ActionResult<IEnumerable<MovieDTO>>> Filter([FromQuery] MovieFilterDTO movieFilterDTO)
        {
            var moviesQueryable = _dbContext.Movies.AsQueryable();

            if (!String.IsNullOrEmpty(movieFilterDTO.Title))
            {
                moviesQueryable = moviesQueryable.Where(m => m.Title.Contains(movieFilterDTO.Title));
            }

            if (movieFilterDTO.inCinemas)
            {
                moviesQueryable = moviesQueryable.Where(m => m.InCinemas);
            }

            if (movieFilterDTO.UpcomingReleases)
            {
                var today = DateTime.Today;
                moviesQueryable = moviesQueryable.Where(m => m.ReleaseDate > today);
            }

            if (movieFilterDTO.GenreId != 0 )
            {
                moviesQueryable = moviesQueryable
                                  .Where(m => m.Genres.Select(g => g.Id).Contains(movieFilterDTO.GenreId));
            }
            else
            {
                return NotFound();
            }

            var movies = await moviesQueryable.Include(m => m.Genres).ToListAsync();

            return _mapper.Map<List<MovieDTO>>(movies);
        }
        */

        [HttpPost]
        public async Task<ActionResult> Post(MovieCreationDTO movieCreationDTO)
        {
            var movie = _mapper.Map<Movie>(movieCreationDTO);

            movie.Genres.ForEach(g => _dbContext.Entry(g).State = EntityState.Unchanged);
            movie.CinemaHalls.ForEach(c => _dbContext.Entry(c).State = EntityState.Unchanged);

            if (movie.MovieActors is not null)
            {
                for (int i = 0; i < movie.MovieActors.Count; i++)
                {
                    movie.MovieActors[i].Order = i + 1;
                }
            }

            _dbContext.Add(movie);
            await _dbContext.SaveChangesAsync();

            return Ok();
        }
    }
}
