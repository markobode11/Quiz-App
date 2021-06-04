using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DAL.App.EF;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using PublicApi.DTO.v1;
using PublicApi.DTO.v1.Enums;

namespace WebApp.ApiControllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AnswerablesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public AnswerablesController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Answerables
        [HttpGet("All/{type}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Answerable>>> GetAnswerablesByType(EType type)
        {
            return await _context.Answerables
                .Where(x => x.Type == _mapper.Map<Domain.App.Enums.EType>(type))
                .Select(x => _mapper.Map<Answerable>(x))
                .ToListAsync();
        }

        // GET: api/Answerables/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Answerable>> GetAnswerable(int id)
        {
            var answerable = await _context.Answerables.FirstOrDefaultAsync(x => x.Id == id);

            if (answerable == null) return NotFound("Bad id!");

            return _mapper.Map<Answerable>(answerable);
        }

        // PUT: api/Answerables/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnswerable(int id, Answerable answerable)
        {
            if (id != answerable.Id) return BadRequest();

            try
            {
                _context.Answerables.Update(_mapper.Map<Domain.App.Answerable>(answerable));
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                return BadRequest(e.Message);
            }

            return NoContent();
        }

        // POST: api/Answerables
        [HttpPost]
        public async Task<ActionResult<Answerable>> PostAnswerable(Answerable answerable)
        {
            try
            {
                var added = _context.Answerables.Add(_mapper.Map<Domain.App.Answerable>(answerable));
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetAnswerable", new {id = added.Entity.Id}, added.Entity);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: api/Answerables/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnswerable(int id)
        {
            var answerable = await _context.Answerables.FindAsync(id);
            if (answerable == null) return NotFound("Bad id!");

            _context.Answerables.Remove(answerable);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("AllQuestions/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<QuestionEdit>>> GetAllQuestionsForAnswerable(int id)
        {
            var answerable = await _context.Answerables
                .Include(x => x.Questions)
                .ThenInclude(x => x.Question)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (answerable == null) return BadRequest("Bad id!");

            return answerable.Questions!
                .Select(question => _mapper.Map<QuestionEdit>(question.Question))
                .ToList();
        }

        [HttpPost("AddExistingQuestion/{answerableId}/{questionId}")]
        public async Task<ActionResult> AddExistingQuestion(int answerableId, int questionId)
        {
            _context.QuestionInAnswerables.Add(new Domain.App.QuestionInAnswerable
            {
                AnswerableId = answerableId,
                QuestionId = questionId
            });

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("AddNewQuestion/{answerableId}")]
        public async Task<ActionResult> AddNewQuestion(int answerableId, QuestionCreate question)
        {
            _context.QuestionInAnswerables.Add(new Domain.App.QuestionInAnswerable
            {
                AnswerableId = answerableId,
                Question = _mapper.Map<Domain.App.Question>(question)
            });

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("RemoveQuestion/{answerableId}/{questionId}")]
        public async Task<ActionResult> RemoveQuestion(int answerableId, int questionId)
        {
            var entity = await _context.QuestionInAnswerables
                .FirstOrDefaultAsync(x => x.AnswerableId == answerableId && x.QuestionId == questionId);

            if (entity == null) return BadRequest("Some id is bad!");

            _context.QuestionInAnswerables.Remove(entity);

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("Statistics")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<AnswerableStatistics>> GetStatistics()
        {
            var scoreDict = new Dictionary<string, List<double>>();

            foreach (var answerable in _context.AppUserAnswerables.Include(x => x.Answerable))
            {
                if (answerable.Answerable!.Type == Domain.App.Enums.EType.Poll) continue;

                var score = (double) answerable.CorrectAnswers /
                    (answerable.CorrectAnswers + answerable.IncorrectAnswers) * 100.0;

                if (scoreDict.ContainsKey(answerable.Answerable.Name))
                {
                    scoreDict[answerable.Answerable.Name].Add(score);
                }
                else
                {
                    scoreDict.Add(answerable.Answerable.Name, new List<double>
                    {
                        score
                    });
                }
            }

            return scoreDict
                .Select(x => new AnswerableStatistics
                {
                    AnswerableName = x.Key,
                    AverageScore = Math.Round(x.Value.Sum() / x.Value.Count)
                })
                .ToList();
        }
    }
}