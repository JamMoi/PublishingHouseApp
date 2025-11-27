using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PublishingHouseApp.Data;
using PublishingHouseApp.Models;

namespace PublishingHouseApp.Controllers
{
    public class BooksController : Controller
    {
        private readonly PublishingHouseContext db;

        public BooksController(PublishingHouseContext context)
        {
            db = context;
        }

        // GET: Books
        public async Task<IActionResult> Index()
        {
            var publishingHouseContext = db.Books.Include(b => b.Author);
            return View(await publishingHouseContext.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await db.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            //ViewData["AuthorId"] = new SelectList(db.Authors, "AuthorId", "Firstname");
            ViewData["AuthorId"] = new SelectList(
            db.Authors.Select(a => new { a.AuthorId, FullName = a.Firstname + " " + a.Surname }),
             "AuthorId", "FullName"
             );
            return View();
        }

        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BookId,AuthorId,Isbn,Title,Editor,PublishedDate,Edition,Genre,Language,Keywords,PageCount,Format,FilePath,CoverImageUrl,Price,IsAvailable")] Book book)
        {
            if (ModelState.IsValid)
            {
                db.Add(book);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Reload dropdown list if validation fails
            ViewData["AuthorId"] = new SelectList(
               db.Authors.Select(a => new { a.AuthorId, FullName = a.Firstname + " " + a.Surname }),
               "AuthorId",
               "FullName",
               book.AuthorId
            );

            return View(book);
            {
         }
}

        
        //public async Task<IActionResult> Create([Bind("BookId,AuthorId,Isbn,Title,Editor,PublishedDate,Edition,Genre,Language,Keywords,PageCount,Format,FilePath,CoverImageUrl,Price,IsAvailable")] Book book)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Add(book);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["AuthorId"] = new SelectList(db.Authors, "AuthorId", "Firstname", book.AuthorId);
        //    return View(book);
        //}

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await db.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            ViewData["AuthorId"] = new SelectList(db.Authors, "AuthorId", "Firstname", book.AuthorId);
            return View(book);
        }

        // POST: Books/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookId,AuthorId,Isbn,Title,Editor,PublishedDate,Edition,Genre,Language,Keywords,PageCount,Format,FilePath,CoverImageUrl,Price,IsAvailable")] Book book)
        {
            if (id != book.BookId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(book);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.BookId))
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
            ViewData["AuthorId"] = new SelectList(
                db.Authors.Select(a => new { a.AuthorId, FullName = a.Firstname + " " + a.Surname }),
               "AuthorId",
               "FullName",
               book.AuthorId);
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await db.Books
                .Include(b => b.Author)
                .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await db.Books.FindAsync(id);
            if (book != null)
            {
                db.Books.Remove(book);
            }

            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return db.Books.Any(e => e.BookId == id);
        }
    }
}
