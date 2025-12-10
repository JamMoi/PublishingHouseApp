//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Rendering;
//using Microsoft.EntityFrameworkCore;
//using PublishingHouseApp.Data;
//using PublishingHouseApp.Models;

//namespace PublishingHouseApp.Controllers
//{
//    public class BooksController : Controller
//    {
//        private readonly PublishingHouseContext db;

//        public BooksController(PublishingHouseContext context)
//        {
//            db = context;
//        }

//        // GET: Books
//        public async Task<IActionResult> Index()
//        {
//            var publishingHouseContext = db.Books.Include(b => b.Author);
//            return View(await publishingHouseContext.ToListAsync());
//        }

//        // GET: Books/Details/5
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var book = await db.Books
//                .Include(b => b.Author)
//                .FirstOrDefaultAsync(m => m.BookId == id);
//            if (book == null)
//            {
//                return NotFound();
//            }

//            return View(book);
//        }

//        // GET: Books/Create
//        public IActionResult Create()
//        {
//            //ViewData["AuthorId"] = new SelectList(db.Authors, "AuthorId", "Firstname");
//            ViewData["AuthorId"] = new SelectList(
//            db.Authors.Select(a => new { a.AuthorId, FullName = a.Firstname + " " + a.Surname }),
//             "AuthorId", "FullName"
//             );
//            return View();
//        }

//        // POST: Books/Create
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Create([Bind("BookId,AuthorId,Isbn,Title,Editor,PublishedDate,Edition,Genre,Language,Keywords,PageCount,Format,FilePath,CoverImageUrl,Price,IsAvailable")] Book book)
//        {
//            if (ModelState.IsValid)
//            {
//                db.Add(book);
//                await db.SaveChangesAsync();
//                return RedirectToAction(nameof(Index));
//            }

//            // Reload dropdown list if validation fails
//            ViewData["AuthorId"] = new SelectList(
//               db.Authors.Select(a => new { a.AuthorId, FullName = a.Firstname + " " + a.Surname }),
//               "AuthorId",
//               "FullName",
//               book.AuthorId
//            );

//            return View(book);

//}

//        //public async Task<IActionResult> Create([Bind("AuthorId,Isbn,Title,Editor,PublishedDate,Edition,Genre,Language,Keywords,PageCount,Format,FilePath,CoverImageUrl,Price,IsAvailable")] Book book)
//        //{
//        //    if (ModelState.IsValid)
//        //    {
//        //        db.Add(book);
//        //        await db.SaveChangesAsync();
//        //        return RedirectToAction(nameof(Index));
//        //    }
//        //    ViewData["AuthorId"] = new SelectList(db.Authors, "AuthorId", "Firstname", book.AuthorId);
//        //    return View(book);
//        //}

//        // GET: Books/Edit/5
//        public async Task<IActionResult> Edit(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var book = await db.Books.FindAsync(id);
//            if (book == null)
//            {
//                return NotFound();
//            }
//            ViewData["AuthorId"] = new SelectList(db.Authors.Select(a => new { a.AuthorId, FullName = a.Firstname + "" + a.Surname}), "AuthorId", "FullName", book.AuthorId);
//            return View(book);
//        }

//        // POST: Books/Edit/5
//        // To protect from overposting attacks, enable the specific properties you want to bind to.
//        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Edit(int id, [Bind("BookId,AuthorId,Isbn,Title,Editor,PublishedDate,Edition,Genre,Language,Keywords,PageCount,Format,FilePath,CoverImageUrl,Price,IsAvailable")] Book book)
//        {
//            if (id != book.BookId)
//            {
//                return NotFound();
//            }

//            if (ModelState.IsValid)
//            {
//                try
//                {
//                    db.Update(book);
//                    await db.SaveChangesAsync();
//                }
//                catch (DbUpdateConcurrencyException)
//                {
//                    if (!BookExists(book.BookId))
//                    {
//                        return NotFound();
//                    }
//                    else
//                    {
//                        throw;
//                    }
//                }
//                return RedirectToAction(nameof(Index));
//            }
//            ViewData["AuthorId"] = new SelectList(
//                db.Authors.Select(a => new { a.AuthorId, FullName = a.Firstname + " " + a.Surname }),
//               "AuthorId",
//               "FullName",
//               book.AuthorId);
//            return View(book);
//        }

//        // GET: Books/Delete/5
//        public async Task<IActionResult> Delete(int? id)
//        {
//            if (id == null)
//            {
//                return NotFound();
//            }

//            var book = await db.Books
//                .Include(b => b.Author)
//                .FirstOrDefaultAsync(m => m.BookId == id);
//            if (book == null)
//            {
//                return NotFound();
//            }

//            return View(book);
//        }

//        // POST: Books/Delete/5
//        [HttpPost, ActionName("Delete")]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> DeleteConfirmed(int id)
//        {
//            var book = await db.Books.FindAsync(id);
//            if (book != null)
//            {
//                db.Books.Remove(book);
//            }

//            await db.SaveChangesAsync();
//            return RedirectToAction(nameof(Index));
//        }

//        private bool BookExists(int id)
//        {
//            return db.Books.Any(e => e.BookId == id);
//        }
//    }
//}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PublishingHouseApp.Data;
using PublishingHouseApp.Models;

public class BooksController : Controller
{
    private readonly PublishingHouseContext db; //    // Tietokantakonteksti, jonka kautta tehdään kyselyt
    public BooksController(PublishingHouseContext context)
    {
        db = context;
    }

    // GET: Books
    // Hakee kaikki kirjat tietokannasta. Include(b => b.Author) lataa myös kirjailijat, jotta näkymässä voidaan näyttää esimerkiksi kirjan tekijän nimi.
    //Palauttaa Index-näkymän listalla.
    public async Task<IActionResult> Index()
    {
        var publishingHouseContext = db.Books.Include(b => b.Author);
        return View(await publishingHouseContext.ToListAsync());
    }

    // GET: Books/Details/5
    // Näyttää yhden kirjan tarkemmat tiedot.
    public async Task<IActionResult> Details(int? id)//Näyttää yksittäisen kirjan tiedot.Jos id puuttuu tai kirjaa ei löydy, palauttaa NotFound().Käyttää Include(a => a.Author) kirjailijan tietojen hakemiseen.
    {
        if (id == null) return NotFound();//jos id ei löydy

        var book = await db.Books// Haetaan kirja ja sen kirjailija
            .Include(b => b.Author)
            .FirstOrDefaultAsync(m => m.BookId == id);

        if (book == null) return NotFound();//jos ei löydy

        return View(book);
    }

    // GET: Books/Create
    // Näyttää kirjan luontilomakkeen ja täyttää kirjailija-dropdownin
    public IActionResult Create() //Näyttää lomakkeen uuden kirjan lisäämiseksi.Täyttää Author-dropdownin, jossa näkyy "Etunimi Sukunimi" (ViewBag.AuthorId).Tämä dropdown auttaa käyttäjää valitsemaan kirjailijan, johon kirja liitetään.
    {
        // Luodaan listaus kirjailijoista dropdownia varten
        ViewBag.AuthorId = new SelectList(
            db.Authors.Select(a => new
            {
                a.AuthorId,
                FullName = a.Firstname + " " + a.Surname
            }),
            "AuthorId",
            "FullName"
        );

        return View();
    }

    // POST: Books/Create
    // Tallentaa uuden kirjan tietokantaan
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("AuthorId,Isbn,Title,Editor,PublishedDate,Edition,Genre,Language,Keywords,PageCount,Format,FilePath,CoverImageUrl,Price,IsAvailable")] Book book)
        //Tallentaa uuden kirjan tietokantaan, jos lomakkeen tiedot ovat validit.Jos validointi epäonnistuu, Author-dropdown rakennetaan uudelleen.Redirecttaa onnistuneen lisäyksen jälkeen Index-näkymään.
    {
        // Tämä blokki tulostaa mahdolliset virheet konsoliin debuggausta varten
        if (!ModelState.IsValid)
        {
            var allErrors = ModelState
                .Where(ms => ms.Value.Errors.Any())
                .Select(ms => new {
                    Field = ms.Key,
                    Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                }).ToList();

            foreach (var error in allErrors)
            {
                Console.WriteLine($"Field: {error.Field}, Errors: {string.Join(", ", error.Errors)}");
            }
        }
        // Jos lomake on validi, tallennetaan kirja
        if (ModelState.IsValid)
        {
            db.Add(book); //BookId luodaan automaattisesti
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        // Jos validointi epäonnistuu, luodaan Author-dropdown uudestaan
        ViewData["AuthorId"] = new SelectList( //lataa Author-dropdown uudelleen, jos validointi epäonnistuu
            db.Authors.Select(a => new { a.AuthorId, FullName = a.Firstname + " " + a.Surname }),
            "AuthorId",
            "FullName",
            book.AuthorId);
       
        return View(book);
    }

    // GET: Books/Edit/5
    //näyttää kirjan muokkausnäkymän
    public async Task<IActionResult> Edit(int? id)//GET hakee kirjan ja näyttää muokkauslomakkeen.POST tallentaa muutokset tietokantaan.
    //Käyttää try/catch DbUpdateConcurrencyExceptionin käsittelyyn, jos kirja poistettiin samaan aikaan, kun sitä muokattiin.
    {
        if (id == null) return NotFound();

        var book = await db.Books.FindAsync(id);//Hakee muokattavan kirjan
        if (book == null) return NotFound();

        //Täyttää Author-dropdownin valmiiksi
        ViewData["AuthorId"] = new SelectList(
            db.Authors.Select(a => new
            {
                a.AuthorId,
                FullName = a.Firstname + " " + a.Surname
            }),
            "AuthorId",
            "FullName",
            book.AuthorId
        );

        return View(book);
    }

    // POST: Books/Edit/5
    //tallentaa muokatun kirjan tiedot tietokantaan
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("BookId,AuthorId,Isbn,Title,Editor,PublishedDate,Edition,Genre,Language,Keywords,PageCount,Format,FilePath,CoverImageUrl,Price,IsAvailable")] Book book)
    {
        if (id != book.BookId) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                db.Update(book);            //Päivittää kirjan tiedot
                await db.SaveChangesAsync(); //Tallentaa muutokset tietokantaan
            }
            catch (DbUpdateConcurrencyException)
            {
                // Jos kirja poistettiin sillä aikaa kun sitä muokattiin
                if (!BookExists(book.BookId))
                    return NotFound();
                else
                    throw;
            }

            return RedirectToAction(nameof(Index));
        }
        // Validointi epäonnistui -> rakennetaan dropdown uudelleen
        ViewData["AuthorId"] = new SelectList(
            db.Authors.Select(a => new { a.AuthorId, FullName = a.Firstname + " " + a.Surname }),
            "AuthorId",
            "FullName",
            book.AuthorId
        );

        return View(book);
    }

    // GET: Books/Delete/5
    //Näyttää kirjan poiston vahvistusnäkymän
    public async Task<IActionResult> Delete(int? id)//GET näyttää vahvistusnäkymän kirjan poistolle.
//POST poistaa kirjan tietokannasta ja redirecttaa Index-näkymään.
    {
        if (id == null) return NotFound();

        // Lataa kirja ja sen kirjailija
        var book = await db.Books
            .Include(b => b.Author)
            .FirstOrDefaultAsync(m => m.BookId == id);

        if (book == null) return NotFound();

        return View(book);
    }

    // POST: Books/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id) //Poistaa kirjan tietokannasta
    {
        var book = await db.Books.FindAsync(id);
        if (book != null)
        {
            db.Books.Remove(book);
        }

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // Tarkistaa tietokannasta, onko kirja olemassa. Käytetään muokkaus- ja poisto-operaatioiden concurrency-tarkistukseen.
    private bool BookExists(int id)
    {
        return db.Books.Any(e => e.BookId == id);
    }
}

