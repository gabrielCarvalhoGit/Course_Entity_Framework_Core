using AutoMapper;
using AutoMapper.QueryableExtensions;
using EFCoreMovies.Date;
using EFCoreMovies.DTO;
using EFCoreMovies.DTO.PostDTOs;
using EFCoreMovies.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreMovies.Controllers
{
    [ApiController]
    [Route("api/Cinema")]
    public class CinemaController : ControllerBase
    {
        private readonly EFCoreDbContext _dbContext;
        private readonly IMapper _mapper;

        public CinemaController(EFCoreDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<CinemaDTO>> Get()
        {
            return await _dbContext.Cinemas.ProjectTo<CinemaDTO>(_mapper.ConfigurationProvider).ToListAsync();
        }

        [HttpGet("CloseToMe")]
        public async Task<ActionResult> Get(double latitude, double longitude)
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var myLocation = geometryFactory.CreatePoint(new Coordinate(longitude, latitude));

            var maxDistanceInMeters = 2000; //2km

            var cinemas = await _dbContext.Cinemas
                .OrderBy(c => c.Location.Distance(myLocation))
                .Where(c => c.Location.IsWithinDistance(myLocation, maxDistanceInMeters))
                //IsWithinDistance => verifica se está dentro da distância calculando a diferença entre os parâmetros
                .Select(c => new
                {
                    Name = c.Name,
                    Distance = Math.Round(c.Location.Distance(myLocation))
                }).ToListAsync();

            return Ok(cinemas);

            //-69.940154, 18.483280
        }

        [HttpPost]
        public async Task<ActionResult> Post()
        {
            var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
            var cinemaLocation = geometryFactory.CreatePoint(new Coordinate(-69.940154, 18.483280));

            var cinema = new Cinema()
            {
                Name = "My Cinema",
                Location = cinemaLocation,
                CinemaOffer = new CinemaOffer()
                {
                    DiscountPercentage = 5,
                    Begin = DateTime.Today,
                    End = DateTime.Today.AddDays(7)
                },
                CinemaHalls = new HashSet<CinemaHall>()
                {
                    new CinemaHall()
                    {
                        Cost = 200,
                        CinemaHallType = CinemaHallType.TwoDimensions
                    },
                    new CinemaHall()
                    {
                        Cost = 250,
                        CinemaHallType = CinemaHallType.ThreeDimensions
                    }
                }
            };

            _dbContext.Add(cinema);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("WithDTO")]
        public async Task<ActionResult> Post(CinemaCreationDTO cinemaCreationDTO)
        {
            var cinema = _mapper.Map<Cinema>(cinemaCreationDTO);
            _dbContext.Add(cinema);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("CinemaOffer")]
        public async Task<ActionResult> PutCinemaOffer(CinemaOffer cinemaOffer)
        {
            _dbContext.Update(cinemaOffer);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(CinemaCreationDTO cinemaCreationDTO, int id)
        {
            var cinemaDB = await _dbContext.Cinemas
                .Include(c => c.CinemaHalls)
                .Include(c => c.CinemaOffer)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cinemaDB is null)
            {
                return NotFound();
            
            }

            cinemaDB = _mapper.Map(cinemaCreationDTO, cinemaDB);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
