using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.App.EF;
using Extensions.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using PublicApi.DTO.v1;

namespace WebApp.ApiControllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AppUserAnswerablesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AppUserAnswerablesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/AppUserAnswerables
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppUserAnswerable>>> GetAppUserAnswerables()
        {
            return await _context.AppUserAnswerables
                .Include(x => x.Answerable)
                .Where(x => x.AppUserId == User.GetUserId())
                .Select(x => _mapper.Map<AppUserAnswerable>(x))
                .ToListAsync();
        }


        // POST: api/AppUserAnswerables
        [HttpPost]
        public async Task<ActionResult<AppUserAnswerable>> PostAppUserAnswerable(
            AppUserAnswerableCreate appUserAnswerable)
        {
            try
            {
                var added = await _context.AppUserAnswerables.AddAsync(new Domain.App.AppUserAnswerable
                {
                    AnswerableId = appUserAnswerable.AnswerableId,
                    AppUserId = User.GetUserId()!.Value,
                    CorrectAnswers = appUserAnswerable.CorrectAnswers,
                    IncorrectAnswers = appUserAnswerable.IncorrectAnswers
                });

                await _context.SaveChangesAsync();

                return Created("/AppUserAnswerables", _mapper.Map<AppUserAnswerable>(added.Entity));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}