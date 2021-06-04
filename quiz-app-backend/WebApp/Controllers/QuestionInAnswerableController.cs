using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.App.EF;
using Domain.App;

namespace WebApp.Controllers
{
    public class QuestionInAnswerableController : Controller
    {
        private readonly AppDbContext _context;

        public QuestionInAnswerableController(AppDbContext context)
        {
            _context = context;
        }

        // GET: QuestionInAnswerable
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.QuestionInAnswerables.Include(q => q.Answerable).Include(q => q.Question);
            return View(await appDbContext.ToListAsync());
        }

        // GET: QuestionInAnswerable/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionInAnswerable = await _context.QuestionInAnswerables
                .Include(q => q.Answerable)
                .Include(q => q.Question)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (questionInAnswerable == null)
            {
                return NotFound();
            }

            return View(questionInAnswerable);
        }

        // GET: QuestionInAnswerable/Create
        public IActionResult Create()
        {
            ViewData["AnswerableId"] = new SelectList(_context.Answerables, "Id", "Description");
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Text");
            return View();
        }

        // POST: QuestionInAnswerable/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,QuestionId,AnswerableId")] QuestionInAnswerable questionInAnswerable)
        {
            if (ModelState.IsValid)
            {
                _context.Add(questionInAnswerable);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AnswerableId"] = new SelectList(_context.Answerables, "Id", "Description", questionInAnswerable.AnswerableId);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Text", questionInAnswerable.QuestionId);
            return View(questionInAnswerable);
        }

        // GET: QuestionInAnswerable/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionInAnswerable = await _context.QuestionInAnswerables.FindAsync(id);
            if (questionInAnswerable == null)
            {
                return NotFound();
            }
            ViewData["AnswerableId"] = new SelectList(_context.Answerables, "Id", "Description", questionInAnswerable.AnswerableId);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Text", questionInAnswerable.QuestionId);
            return View(questionInAnswerable);
        }

        // POST: QuestionInAnswerable/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,QuestionId,AnswerableId")] QuestionInAnswerable questionInAnswerable)
        {
            if (id != questionInAnswerable.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(questionInAnswerable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionInAnswerableExists(questionInAnswerable.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AnswerableId"] = new SelectList(_context.Answerables, "Id", "Description", questionInAnswerable.AnswerableId);
            ViewData["QuestionId"] = new SelectList(_context.Questions, "Id", "Text", questionInAnswerable.QuestionId);
            return View(questionInAnswerable);
        }

        // GET: QuestionInAnswerable/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionInAnswerable = await _context.QuestionInAnswerables
                .Include(q => q.Answerable)
                .Include(q => q.Question)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (questionInAnswerable == null)
            {
                return NotFound();
            }

            return View(questionInAnswerable);
        }

        // POST: QuestionInAnswerable/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var questionInAnswerable = await _context.QuestionInAnswerables.FindAsync(id);
            _context.QuestionInAnswerables.Remove(questionInAnswerable);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionInAnswerableExists(int id)
        {
            return _context.QuestionInAnswerables.Any(e => e.Id == id);
        }
    }
}
