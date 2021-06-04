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
    public class AppUserAnswerableController : Controller
    {
        private readonly AppDbContext _context;

        public AppUserAnswerableController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AppUserAnswerable
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.AppUserAnswerables.Include(a => a.Answerable).Include(a => a.AppUser);
            return View(await appDbContext.ToListAsync());
        }

        // GET: AppUserAnswerable/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUserAnswerable = await _context.AppUserAnswerables
                .Include(a => a.Answerable)
                .Include(a => a.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appUserAnswerable == null)
            {
                return NotFound();
            }

            return View(appUserAnswerable);
        }

        // GET: AppUserAnswerable/Create
        public IActionResult Create()
        {
            ViewData["AnswerableId"] = new SelectList(_context.Answerables, "Id", "Description");
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Firstname");
            return View();
        }

        // POST: AppUserAnswerable/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AppUserId,AnswerableId,CorrectAnswers,IncorrectAnswers")] AppUserAnswerable appUserAnswerable)
        {
            if (ModelState.IsValid)
            {
                _context.Add(appUserAnswerable);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AnswerableId"] = new SelectList(_context.Answerables, "Id", "Description", appUserAnswerable.AnswerableId);
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Firstname", appUserAnswerable.AppUserId);
            return View(appUserAnswerable);
        }

        // GET: AppUserAnswerable/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUserAnswerable = await _context.AppUserAnswerables.FindAsync(id);
            if (appUserAnswerable == null)
            {
                return NotFound();
            }
            ViewData["AnswerableId"] = new SelectList(_context.Answerables, "Id", "Description", appUserAnswerable.AnswerableId);
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Firstname", appUserAnswerable.AppUserId);
            return View(appUserAnswerable);
        }

        // POST: AppUserAnswerable/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppUserId,AnswerableId,CorrectAnswers,IncorrectAnswers")] AppUserAnswerable appUserAnswerable)
        {
            if (id != appUserAnswerable.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(appUserAnswerable);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AppUserAnswerableExists(appUserAnswerable.Id))
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
            ViewData["AnswerableId"] = new SelectList(_context.Answerables, "Id", "Description", appUserAnswerable.AnswerableId);
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Firstname", appUserAnswerable.AppUserId);
            return View(appUserAnswerable);
        }

        // GET: AppUserAnswerable/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var appUserAnswerable = await _context.AppUserAnswerables
                .Include(a => a.Answerable)
                .Include(a => a.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (appUserAnswerable == null)
            {
                return NotFound();
            }

            return View(appUserAnswerable);
        }

        // POST: AppUserAnswerable/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var appUserAnswerable = await _context.AppUserAnswerables.FindAsync(id);
            _context.AppUserAnswerables.Remove(appUserAnswerable);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AppUserAnswerableExists(int id)
        {
            return _context.AppUserAnswerables.Any(e => e.Id == id);
        }
    }
}
