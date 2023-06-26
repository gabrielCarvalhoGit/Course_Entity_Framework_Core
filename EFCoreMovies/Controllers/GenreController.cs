using AutoMapper;
using EFCoreMovies.Date;
using EFCoreMovies.DTO.PostDTOs;
using EFCoreMovies.Entities;
using EFCoreMovies.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace EFCoreMovies.Controllers
{
    [ApiController]
    [Route("api/Genre")]
    public class GenreController : ControllerBase
    {
        private readonly EFCoreDbContext _dbContext;
        private readonly IMapper _mapper;

        public GenreController(EFCoreDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IEnumerable<Genre>> Get()
        {
            return await _dbContext.Genres.AsNoTracking()
                .OrderBy(g => g.Name)
                .ToListAsync();

            #region coments
            //order by - ordena os registros de acordo com o filtro
            //Take - Quantidade de registros que deverá ser apresentado
            //Skip - Pula a quantidade de registros passados no parâmetro
            #endregion
        }

        [HttpPost("Add2")]
        public async Task<ActionResult> PostAdd(int id)
        {
            var genre = await _dbContext.Genres.FirstOrDefaultAsync(g => g.Id == id);

            if (genre is null)
            {
                return NotFound();
            }

            genre.Name += "2";
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> Post(GenreCreationDTO genreCreationDTO)
        {
            var genre = _mapper.Map<Genre>(genreCreationDTO);
            var status1 = _dbContext.Entry(genre).State;

            _dbContext.Add(genre);
            var status2 = _dbContext.Entry(genre).State;

            await _dbContext.SaveChangesAsync();
            var status3 = _dbContext.Entry(genre).State;

            return Ok();
        }

        [HttpPost("SeveralRecords")]
        public async Task<ActionResult> Post(GenreCreationDTO[] genreCreationDTO)
        {
            var genres = _mapper.Map<Genre[]>(genreCreationDTO);

            _dbContext.AddRange(genres);

            await _dbContext.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var genre = await _dbContext.Genres.FirstOrDefaultAsync(p => p.Id == id);

            if (genre is null)
            {
                return NotFound();
            }

            _dbContext.Remove(genre);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("SoftDelete{id:int}")]
        public async Task<ActionResult> SoftDelete(int id)
        {
            var genre = await _dbContext.Genres.FirstOrDefaultAsync(p => p.Id == id);

            if (genre is null)
            {
                return NotFound();
            }

            genre.IsDeleted = true;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("Restore{id:int}")]
        public async Task<ActionResult> Restore(int id)
        {
            var genre = await _dbContext.Genres.IgnoreQueryFilters().FirstOrDefaultAsync(p => p.Id == id);

            if (genre is null)
            {
                return NotFound();
            }

            genre.IsDeleted = false;
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
