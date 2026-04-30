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
using Microsoft.AspNetCore.Authorization;
using Microsoft.CodeAnalysis.CSharp.Syntax; // Mahdollistaa roolipohjaisen suojauksen
using PublishingHouseApp.ViewModels;

namespace PublishingHouseApp.Controllers
{

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
            //var publishingHouseContext = db.Books.Include(b => b.Author);
            //return View(await publishingHouseContext.ToListAsync());
            var publishingHouseContext = db.Books
            .Include(b => b.Author)
            .Include(b => b.BookGenres)
                .ThenInclude(bg => bg.Genre);

            return View(await publishingHouseContext.ToListAsync());

        }

        // GET: Books/Details/5
        // Näyttää yhden kirjan tarkemmat tiedot.
        public async Task<IActionResult> Details(int? id, string? returnAction)// Näyttää yhden kirjan tarkemmat tiedot.
                                                                               // id kertoo, mikä kirja haetaan tietokannasta.
                                                                               // returnAction kertoo, mistä näkymästä käyttäjä tuli
                                                                               // (esimerkiksi "Browse" tai "Index"), jotta Back-painike voidaan ohjata oikein
        {
            if (id == null) return NotFound();//Varmistaa että ID on olemassa

            //var book = await db.Books// Haetaan yksi kirja ja sen kirjoittaja
            //    .Include(b => b.Author)
            //    .FirstOrDefaultAsync(m => m.BookId == id);
            var book = await db.Books
        .Include(b => b.Author)
        .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
        .FirstOrDefaultAsync(m => m.BookId == id);

            if (book == null) return NotFound();//jos kirjaa ei löydy palauttaa NotFound()
            ViewBag.ReturnAction = returnAction;
            return View(book); //Palauttaa kirjan tiedot näkymään ( Lähettää yhden Book-olion näkymään)

        }

        // GET: Books/Create
        // Näyttää kirjan luontilomakkeen ja täyttää kirjailija-dropdownin
        //Vain admin pääsee tähän näkymään
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewBag.AuthorId = new SelectList(
                db.Authors.Select(a => new
                {
                    a.AuthorId,
                    FullName = a.Firstname + " " + a.Surname
                }),
                "AuthorId",
                "FullName"
            );

            var viewModel = new BookCreateViewModel
            {
                AvailableGenres = db.Genres.OrderBy(g => g.Name).ToList()
            };

            return View(viewModel);
        }
        //public IActionResult Create() //Näyttää lomakkeen uuden kirjan lisäämiseksi.Täyttää Author-dropdownin, jossa näkyy "Etunimi Sukunimi" (ViewBag.AuthorId).Tämä dropdown auttaa käyttäjää valitsemaan kirjailijan, johon kirja liitetään.
        //{
        //    //Luo dropdown-listaus kirjailijoista, jossa näytetään etunimi ja sukunimi yhdessä
        //    ViewBag.AuthorId = new SelectList(
        //        db.Authors.Select(a => new
        //        {
        //            a.AuthorId,
        //            FullName = a.Firstname + " " + a.Surname
        //        }),
        //        "AuthorId",
        //        "FullName"
        //    );
        //    return View();
        //}

        // POST: Books/Create
        // Tallentaa uuden kirjan tietokantaan, vain admin voi lisätä uusia kirjoja. Varmistaa, että lomakkeen tiedot ovat validit. Jos validointi epäonnistuu, Author-dropdown rakennetaan uudelleen. Redirecttaa onnistuneen lisäyksen jälkeen Index-näkymään.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(BookCreateViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.AuthorId = new SelectList(
                    db.Authors.Select(a => new
                    {
                        a.AuthorId,
                        FullName = a.Firstname + " " + a.Surname
                    }),
                    "AuthorId",
                    "FullName",
                    viewModel.Book.AuthorId
                );

                viewModel.AvailableGenres = db.Genres.ToList();
                return View(viewModel);
            }

            foreach (var genreId in viewModel.SelectedGenreIds)
            {
                viewModel.Book.BookGenres.Add(new BookGenre
                {
                    GenreId = genreId
                });
            }

            db.Books.Add(viewModel.Book);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> Create([Bind("AuthorId,Isbn,Title,Editor,PublishedDate,Edition,Genre,Language,Keywords,PageCount,Format,FilePath,CoverImageUrl,Price,IsAvailable")] Book book)
        ////Tallentaa uuden kirjan tietokantaan, jos lomakkeen tiedot ovat validit.Jos validointi epäonnistuu, Author-dropdown rakennetaan uudelleen.Redirecttaa onnistuneen lisäyksen jälkeen Index-näkymään.
        //{
        //    // Tämä blokki tulostaa mahdolliset virheet konsoliin debuggausta varten
        //    if (!ModelState.IsValid)
        //    {
        //        var allErrors = ModelState
        //            .Where(ms => ms.Value.Errors.Any())
        //            .Select(ms => new
        //            {
        //                Field = ms.Key,
        //                Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToArray()
        //            }).ToList();

        //        foreach (var error in allErrors)
        //        {
        //            Console.WriteLine($"Field: {error.Field}, Errors: {string.Join(", ", error.Errors)}");
        //        }
        //    }
        //    // Jos lomake on validi, tallennetaan kirja ja ohjaa takaisin listaan
        //    if (ModelState.IsValid)
        //    {
        //        db.Add(book);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    // Jos validointi epäonnistuu, luodaan Author-dropdown uudestaan
        //    ViewData["AuthorId"] = new SelectList( //lataa Author-dropdown uudelleen, jos validointi epäonnistuu
        //        db.Authors.Select(a => new { a.AuthorId, FullName = a.Firstname + " " + a.Surname }),
        //        "AuthorId",
        //        "FullName",
        //        book.AuthorId);

        //    return View(book);
        //}

        // GET: Books/Edit/5 //käyttäjä avaa selaimessa kirjan muokkausnäkymän
        //näyttää kirjan muokkausnäkymän
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)//HTTP GET- metodi, joka ajetaan kun käyttäjä menee osoitteeseen /Books/Edit/5,  hakee kirjan ja näyttää muokkauslomakkeen.POST tallentaa muutokset tietokantaan.
                                                      // lähettää tiedot Viewille //Käyttää try/catch DbUpdateConcurrencyExceptionin käsittelyyn, jos kirja poistettiin samaan aikaan, kun sitä muokattiin.
        {
            if (id == null) 
                return NotFound(); //estää virheellisen URL käytön, Jos käyttäjä menee /Books/Edit ilman id:tä, palauttaa NotFound()-vastauksen.

            var book = await db.Books //hakee tietokannasta kirjan, jonka BookId vastaa annettua idt

                .Include(b => b.BookGenres) //Include (b =>b.BooKgenres lataa samalla kirjan Genreliitokset, jptta voidaan näyttää valmiiksi valitut genret checkboxesissa
                .FirstOrDefaultAsync(b => b.BookId == id);

            // Jos kirjaa ei löydy tietokannasta, palautetaan 404
            if (book == null)
                return NotFound();
            // Luodaan Author-dropdownia varten lista kirjailijoista.
            // Jokaisesta kirjailijasta muodostetaan FullName = "Firstname + Surname".
            // Viimeinen parametri (book.AuthorId) määrittää valmiiksi valitun kirjailijan.

            ViewData["AuthorId"] = new SelectList(
                db.Authors.Select(a => new
                {
                    a.AuthorId,
                    FullName = a.Firstname + "" + a.Surname
                }),
                "AuthorId", // value kenttä dropdownissa
                "FullName", // käyttäjälle näkyvä teksti
                book.AuthorId // valmiiksi valittu kirjailija

                );
            // Luodaan näkymälle ViewModel, joka sisältää:
            // - itse kirjan tiedot
            // - kaikki mahdolliset genret aakkosjärjestyksessä
            // - listan niistä genreistä, jotka kirjalla jo on
            var viewModel = new BookEditViewModel
            {
                Book = book,         // Lähetetään kirjan tiedot näkymälle

                // Haetaan kaikki genret aakkosjärjestyksessä dropdownia varten

                AvailableGenres = db.Genres
                .OrderBy(g => g.Name)
                .ToList(),

                // Poimitaan kirjan nykyiset genre-ID:t listaan.
                // Näiden avulla checkboxit voidaan näyttää valmiiksi valittuina.
                SelectedGenreIds = book.BookGenres
            .Select(bg => bg.GenreId)
            .ToList()
            };
            // Lähetetään valmis ViewModel Edit-näkymälle.
            return View(viewModel);
        }


        //    var book = await db.Books.FindAsync(id);//EF Core tekee SQL-kyselyn:Hakee muokattavan kirjan
        //    if (book == null) return NotFound();//varmistaa että kirja löytyy

        //    //SelectListin luonti kirjailijoista dropdownia varten
        //    ViewData["AuthorId"] = new SelectList(//EF Core hakee kaikki kirjailijat tietokannasta ja luo SelectListin, jota käytetään dropdownin täyttämiseen näkymässä.
        //        db.Authors.Select(a => new
        //        {
        //            a.AuthorId,
        //            FullName = a.Firstname + " " + a.Surname
        //        }),
        //        "AuthorId",
        //        "FullName",
        //        book.AuthorId
        //    );

        //    return View(book);
        //}

        // POST: Books/Edit/5
        // Tallentaa muokatun kirjan tiedot tietokantaan.
        // Vain Admin-roolin käyttäjät saavat muokata kirjoja.
        [HttpPost]
        [ValidateAntiForgeryToken] // Suojaa lomaketta CSRF-hyökkäyksiltä
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, BookEditViewModel viewModel)
        {
            // Varmistetaan, että URL:ssa tullut id vastaa lomakkeelta tullutta BookId:tä.
            // Jos ne eivät täsmää, käyttäjä yrittää muokata väärää kirjaa.
            if (id != viewModel.Book.BookId)
                return NotFound();

            // Tarkistetaan, että lomakkeen tiedot ovat validit.
            // Jos esim. jokin pakollinen kenttä puuttuu, mennään takaisin lomakkeeseen.
            if (!ModelState.IsValid)
            {
                // Rakennetaan kirjailija-dropdown uudelleen,
                // koska muuten se katoaisi näkymästä virhetilanteessa.
                ViewData["AuthorId"] = new SelectList(
                    db.Authors.Select(a => new
                    {
                        a.AuthorId,
                        FullName = a.Firstname + " " + a.Surname
                    }),
                    "AuthorId",               // dropdownin value
                    "FullName",               // dropdownissa näkyvä teksti
                    viewModel.Book.AuthorId   // valmiiksi valittu kirjailija
                );

                // Haetaan genrelista uudelleen aakkosjärjestyksessä,
                // jotta checkboxit näkyvät uudelleen virhetilanteessa.
                viewModel.AvailableGenres = db.Genres
                    .OrderBy(g => g.Name)
                    .ToList();

                // Palautetaan sama näkymä käyttäjälle korjattavaksi.
                return View(viewModel);
            }

            // Haetaan tietokannasta oikea kirja, jota ollaan muokkaamassa.
            // Include(BookGenres) tarvitaan, jotta vanhat genreliitokset voidaan poistaa ja lisätä uudelleen.
            var bookFromDb = await db.Books
                .Include(b => b.BookGenres)
                .FirstOrDefaultAsync(b => b.BookId == id);

            // Jos kirjaa ei enää löydy tietokannasta, palautetaan 404.
            if (bookFromDb == null)
                return NotFound();

            // Päivitetään kirjan tavalliset tiedot lomakkeelta tulleilla arvoilla.
            bookFromDb.AuthorId = viewModel.Book.AuthorId;
            bookFromDb.Isbn = viewModel.Book.Isbn;
            bookFromDb.Title = viewModel.Book.Title;
            bookFromDb.Editor = viewModel.Book.Editor;
            bookFromDb.PublishedDate = viewModel.Book.PublishedDate;
            bookFromDb.Edition = viewModel.Book.Edition;
            bookFromDb.Language = viewModel.Book.Language;
            bookFromDb.Keywords = viewModel.Book.Keywords;
            bookFromDb.PageCount = viewModel.Book.PageCount;
            bookFromDb.Format = viewModel.Book.Format;
            bookFromDb.FilePath = viewModel.Book.FilePath;
            bookFromDb.CoverImageUrl = viewModel.Book.CoverImageUrl;
            bookFromDb.Price = viewModel.Book.Price;
            bookFromDb.IsAvailable = viewModel.Book.IsAvailable;

            // Poistetaan kaikki kirjan vanhat genreliitokset.
            // Näin voidaan rakentaa uusi lista käyttäjän valintojen perusteella.
            bookFromDb.BookGenres.Clear();

            // Käydään läpi kaikki käyttäjän valitsemat genre-ID:t.
            foreach (var genreId in viewModel.SelectedGenreIds)
            {
                // Lisätään jokaisesta valitusta genrestä uusi BookGenre-liitos.
                bookFromDb.BookGenres.Add(new BookGenre
                {
                    BookId = bookFromDb.BookId,
                    GenreId = genreId
                });
            }

            // Tallennetaan kaikki muutokset tietokantaan.
            await db.SaveChangesAsync();

            // Kun tallennus onnistuu, siirrytään takaisin kirjojen listaan.
            return RedirectToAction(nameof(Index));
        }
        // GET: Books/Delete/5
        //Näyttää kirjan poiston vahvistusnäkymän
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)//GET näyttää vahvistusnäkymän kirjan poistolle.
                                                        //POST poistaa kirjan tietokannasta ja redirecttaa Index-näkymään.
        {
            if (id == null) return NotFound();

            var book = await db.Books
        .Include(b => b.Author)
        .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
        .FirstOrDefaultAsync(m => m.BookId == id);
            if (book == null) return NotFound();

            return View(book);
        }

        // POST: Books/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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

        //GET: Books/Browse
        //Näyttää kaikille käyttäjille kirjanäkymän kortteina
        public async Task<IActionResult> Browse()
        {
            //Haetaan kaikki tietokannan kirjat
            // Include(b => b.Author) lataa mukaan kirjailijan tiedot
            //Include + theninclude lataa mukaan kirjan genret
            var books = await db.Books
                .Include(b => b.Author)
                .Include(b => b.BookGenres)
                .ThenInclude(bg => bg.Genre)
                .OrderBy(b => b.Title)
                .ToListAsync();

            // lähettää kirjalistan Browse-näkymään
            return View(books);
        }

        
        //GET: Book/Search?query=some+search+term
        public async Task<IActionResult> Search(string query) //Hakee kirjoja, joiden otsikko tai ISBN sisältää hakutermin
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction(nameof(Index)); //Jos hakutermi on tyhjä, ohjaa takaisin listaan
            }
            var results = await db.Books
        .Include(b => b.Author)
        .Include(b => b.BookGenres)
            .ThenInclude(bg => bg.Genre)
        .Where(b => b.Title.Contains(query) || b.Isbn.Contains(query))
        .ToListAsync();
            return View("Index", results); //Näytä hakutulokset Index-näkymässä
        }
    }
}