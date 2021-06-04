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
    public class AnswerableController : Controller
    {
        private readonly AppDbContext _context;

        public AnswerableController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Answerable
        public async Task<IActionResult> Index()
        {
            return View(await _context.Answerables.ToListAsync());
        }

        // GET: Answerable/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var answerable = await _context.Answerables
                .FirstOrDefaultAsync(m => m.Id == id);
            if (answerable == null)
            {
                return NotFound();
            }

            return View(answerable);
        }

        // GET: Answerable/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Answerable/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Type")] Answerable answerable)
        {
            if (ModelState.IsValid)
            {
                _context.Add(answerable);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(answerable);
        }

        // GET: Answerable/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var answerable = await _context.Answerables.FindAsync(id);
            if (answerable == null)
            {
                return NotFound();
            }
            return View(answerable);
        }

        // POST: Answerable/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Type")] Answerable answerable)
        {
            if (id != answerable.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(answerable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnswerableExists(answerable.Id))
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
            return View(answerable);
        }

        // GET: Answerable/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var answerable = await _context.Answerables
                .FirstOrDefaultAsync(m => m.Id == id);
            if (answerable == null)
            {
                return NotFound();
            }

            return View(answerable);
        }

        // POST: Answerable/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var answerable = await _context.Answerables.FindAsync(id);
            _context.Answerables.Remove(answerable);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnswerableExists(int id)
        {
            return _context.Answerables.Any(e => e.Id == id);
        }
    }
}
