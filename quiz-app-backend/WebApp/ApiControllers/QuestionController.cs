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

namespace WebApp.ApiControllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [Authorize(Roles = "Admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class QuestionsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;

        public QuestionsController(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Questions
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Question>>> GetQuestions()
        {
            return await _context.Questions
                .Include(x => x.Answers)
                .Select(x => _mapper.Map<Question>(x))
                .ToListAsync();
        }
        
        // GET: api/Questions/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Question>> GetQuestion(int id)
        {
            var question = await _context.Questions
                .Include(x => x.Answers)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (question == null) return NotFound("Bad id!");

            return _mapper.Map<Question>(question);
        }

        // PUT: api/Questions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuestion(int id, QuestionEdit question)
        {
            if (id != question.Id) return BadRequest("Bad id in request!");

            var entityToUpdate = await _context.Questions.FirstOrDefaultAsync(x => x.Id == id);

            if (entityToUpdate == null) return BadRequest("No such question!");

            entityToUpdate.Text = question.Text;

            try
            {
                _context.Questions.Update(entityToUpdate);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException e)
            {
                return BadRequest(e.Message);
            }

            return NoContent();
        }

        // POST: api/Questions
        [HttpPost]
        public async Task<ActionResult<Question>> PostQuestion(QuestionCreate question)
        {
            try
            {
                var added = _context.Questions.Add(_mapper.Map<Domain.App.Question>(question));
                await _context.SaveChangesAsync();
                return Created("/Questions", _mapper.Map<QuestionEdit>(added.Entity));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // DELETE: api/Question/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _context.Questions.FindAsync(id);
            if (question == null) return NotFound("Bad id!");

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpGet("QuestionAnswers/{questionId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Answer>>> GetAllAnswersForQuestion(int questionId)
        {
            return await _context.Answers
                .Where(x => x.QuestionId == questionId)
                .Select(x => _mapper.Map<Answer>(x))
                .ToListAsync();
        }

        [HttpPost("AddAnswer/{questionId}")]
        public async Task<ActionResult> AddAnswerForQuestion(AnswerCreate dto, int questionId)
        {
            try
            {
                await _context.Answers.AddAsync(new Domain.App.Answer
                {
                    IsCorrect = dto.IsCorrect,
                    Text = dto.Text,
                    QuestionId = questionId
                });
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            return NoContent();
        }

        [HttpPost("RemoveAnswer/{answerId}")]
        public async Task<ActionResult> RemoveAnswerFromQuestion(int answerId)
        {
            var answer = await _context.Answers.FirstOrDefaultAsync(x => x.Id == answerId);

            if (answer == null) return BadRequest("Bad id for answer!");

            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}