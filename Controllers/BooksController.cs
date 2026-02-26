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

public class BooksController : Controller //Tämä luo BooksController-luokan, joka perii ASP.NET Core MVC:n Controller-luokan ominaisuudet.
{
    private readonly PublishingHouseContext db; // Tietokantayhteys, jonka kautta tehdään kyselyt
    public BooksController(PublishingHouseContext context) //Konstruktori, joka saa tietokantakontekstin injektoituna, Dependency Injection -periaatteen mukaisesti.
    {
        db = context;
    }

    // GET: Books

    // Hakee kaikki kirjat tietokannasta. Include(b => b.Author) lataa myös kirjailijat, jotta näkymässä voidaan näyttää esimerkiksi kirjan tekijän nimi.
    //Palauttaa kaikkien kirjojen Index-näkymän listalla.
    public async Task<IActionResult> Index()
    {
        var publishingHouseContext = db.Books.Include(b => b.Author);
        return View(await publishingHouseContext.ToListAsync());
    }

    // GET: Books/Details/5
    // Näyttää yhden kirjan tarkemmat tiedot.
    public async Task<IActionResult> Details(int? id)//Näyttää yksittäisen kirjan tiedot.Jos id puuttuu tai kirjaa ei löydy, palauttaa NotFound().Käyttää Include(a => a.Author) kirjailijan tietojen hakemiseen.
    {
        if (id == null) return NotFound();//Varmistaa että ID on olemassa

        var book = await db.Books// Haetaan yksi kirja ja sen kirjoittaja
            .Include(b => b.Author)
            .FirstOrDefaultAsync(m => m.BookId == id);

        if (book == null) return NotFound();//jos kirjaa ei löydy palauttaa NotFound()

        return View(book); //Palauttaa kirjan tiedot näkymään ( Lähettää yhden Book-olion näkymään)

    }

    // GET: Books/Create
    // Näyttää kirjan luontilomakkeen ja täyttää kirjailija-dropdownin
    public IActionResult Create() //Näyttää lomakkeen uuden kirjan lisäämiseksi.Täyttää Author-dropdownin, jossa näkyy "Etunimi Sukunimi" (ViewBag.AuthorId).Tämä dropdown auttaa käyttäjää valitsemaan kirjailijan, johon kirja liitetään.
    {
        // Luo listaus kirjailijoista dropdownia varten
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
        // Jos lomake on validi, tallennetaan kirja ja ohjaa takaisin listaan
        if (ModelState.IsValid)
        {
            db.Add(book);
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

    // GET: Books/Edit/5 //käyttäjä avaa selaimessa kirjan muokkausnäkymän
    //näyttää kirjan muokkausnäkymän
    public async Task<IActionResult> Edit(int? id)//HTTP GET- metodi, joka ajetaan kun käyttäjä menee osoitteeseen /Books/Edit/5,  hakee kirjan ja näyttää muokkauslomakkeen.POST tallentaa muutokset tietokantaan.
   // lähettää tiedot Viewille //Käyttää try/catch DbUpdateConcurrencyExceptionin käsittelyyn, jos kirja poistettiin samaan aikaan, kun sitä muokattiin.
    {
        if (id == null) return NotFound(); //estää virheellisen URL käytön, Jos käyttäjä menee /Books/Edit ilman id:tä, palauttaa NotFound()-vastauksen.

        var book = await db.Books.FindAsync(id);//EF Core tekee SQL-kyselyn:Hakee muokattavan kirjan
        if (book == null) return NotFound();//varmistaa että kirja löytyy

        //SelectListin luonti kirjailijoista dropdownia varten
        ViewData["AuthorId"] = new SelectList(//EF Core hakee kaikki kirjailijat tietokannasta ja luo SelectListin, jota käytetään dropdownin täyttämiseen näkymässä.
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
    public async Task<IActionResult> Edit(int id, [Bind("BookId,AuthorId,Isbn,Title,Editor,PublishedDate,Edition,Genre,Language,Keywords,PageCount,Format,FilePath,CoverImageUrl,Price,IsAvailable")] Book book)//id → tulee URL:stä (/Books/Edit/5)
    //Tässä vaiheessa book.AuthorId sisältää alasvetovalikosta valitun kirjailijan id:n
    {
        if (id != book.BookId) return NotFound();//Varmistaa, ettei URL:n id ja lomakkeelta tullut BookId ole ristiriidassa

        if (ModelState.IsValid)//Tarkistaa, että lomakkeen tiedot ovat validit
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
        ViewData["AuthorId"] = new SelectList( //lähettää author dropdownin uudelleen Viewille jos validointi epäonnistuu
            db.Authors.Select(a => new { a.AuthorId, FullName = a.Firstname + " " + a.Surname }),//db.Authors hakee kaikki kirjailijat tietokannasta, new {AuthorId, FullName} luo uuden anonyymin objektin, jossa on AuthorId ja FullName (etunimi + sukunimi).
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

    // Tarkistaa tietokannasta, onko kirja olemassa. Käyttää muokkaus- ja poisto-operaatioiden concurrency-tarkistukseen.
    private bool BookExists(int id)
    {
        return db.Books.Any(e => e.BookId == id);
    }
}

