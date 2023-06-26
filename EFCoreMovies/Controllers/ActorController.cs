using AutoMapper;
using AutoMapper.QueryableExtensions;
using EFCoreMovies.Date;
using EFCoreMovies.DTO;
using EFCoreMovies.DTO.PostDTOs;
using EFCoreMovies.Entities;
using EFCoreMovies.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EFCoreMovies.Controllers
{
    [ApiController]
    [Route("api/Actors")]
    public class ActorController : ControllerBase
    {
        private readonly EFCoreDbContext _dbContext;
        private readonly IMapper _mapper;

        public ActorController(EFCoreDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<ActorDTO>> Get()
        {
            return await _dbContext.Actors.AsNoTracking()
                .OrderBy(g => g.Name)
                .ProjectTo<ActorDTO>(_mapper.ConfigurationProvider)
                //.Select(a => new ActorDTO { Id = a.Id, Name = a.Name, DateOfBirth = a.DateOfBirth })
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult> Post(ActorCreationDTO actorCreationDTO)
        {
            var actor = _mapper.Map<Actor>(actorCreationDTO);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Put(ActorCreationDTO actorCreationDTO, int id)
        {
            var actorDB = await _dbContext.Actors.FirstOrDefaultAsync(p => p.Id == id);

            if (actorDB is null)
            {
                return NotFound();
            }

            actorDB = _mapper.Map(actorCreationDTO, actorDB);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("Disconnected/{id:int}")]
        public async Task<ActionResult> PutDisconnected(ActorCreationDTO actorCreationDTO, int id)
        {
            var existActor = await _dbContext.Actors.AnyAsync(p => p.Id == id);

            if (!existActor)
            {
                return NotFound();
            }

            var actor = _mapper.Map<Actor>(actorCreationDTO);
            actor.Id = id;

            _dbContext.Update(actor);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
