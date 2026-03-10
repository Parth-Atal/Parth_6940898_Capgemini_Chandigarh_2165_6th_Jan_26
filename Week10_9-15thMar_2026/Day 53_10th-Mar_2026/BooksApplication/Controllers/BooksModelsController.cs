using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BooksApplication.Models;

namespace BooksApplication.Controllers
{
    public class BooksModelsController : Controller
    {
        private readonly BookdbContext _context;

        public BooksModelsController(BookdbContext context)
        {
            _context = context;
        }

        // GET: BooksModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.books.ToListAsync());
        }

        // GET: BooksModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booksModel = await _context.books
                .FirstOrDefaultAsync(m => m.BookModelId == id);
            if (booksModel == null)
            {
                return NotFound();
            }

            return View(booksModel);
        }

        // GET: BooksModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: BooksModels/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookModelId,BookName,AuthorName")] BooksModel booksModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(booksModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(booksModel);
        }

        // GET: BooksModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booksModel = await _context.books.FindAsync(id);
            if (booksModel == null)
            {
                return NotFound();
            }
            return View(booksModel);
        }

        // POST: BooksModels/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookModelId,BookName,AuthorName")] BooksModel booksModel)
        {
            if (id != booksModel.BookModelId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booksModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BooksModelExists(booksModel.BookModelId))
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
            return View(booksModel);
        }

        // GET: BooksModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booksModel = await _context.books
                .FirstOrDefaultAsync(m => m.BookModelId == id);
            if (booksModel == null)
            {
                return NotFound();
            }

            return View(booksModel);
        }

        // POST: BooksModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booksModel = await _context.books.FindAsync(id);
            if (booksModel != null)
            {
                _context.books.Remove(booksModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BooksModelExists(int id)
        {
            return _context.books.Any(e => e.BookModelId == id);
        }
    }
}
