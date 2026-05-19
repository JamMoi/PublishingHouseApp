using Microsoft.AspNetCore.Mvc; // MVC Controller- ja IActionResult-luokat
using Microsoft.AspNetCore.Mvc.Rendering; // SelectList dropdown-listoja varten
using Microsoft.EntityFrameworkCore; // Entity Framework Core: Include, ThenInclude, ToListAsync jne.
using PublishingHouseApp.Data; // PublishingHouseContext eli tietokantayhteys
using PublishingHouseApp.Models; // Book, Author, Genre ja muut mallit
using Microsoft.AspNetCore.Authorization; // Authorize-attribuutti roolien tarkistamiseen
using PublishingHouseApp.ViewModels; // BookCreateViewModel ja BookEditViewModel

namespace PublishingHouseApp.Controllers
{
    // BooksController käsittelee kirjoihin liittyvät pyynnöt:
    // listaus, details, create, edit, delete, browse, search ja live search.
    public class BooksController : Controller
    {
        // db on tietokantayhteys
        // Sen kautta haetaan ja tallennetaan kirjoja, kirjailijoita ja genrejä.
        private readonly PublishingHouseContext db;

        // Konstruktori saa tietokantayhteyden Dependency Injectionin kautta.
        // ASP.NET Core antaa PublishingHouseContext-olion automaattisesti.
        public BooksController(PublishingHouseContext context)
        {
            db = context;
        }

        // GET: Books
        // Tämä action suoritetaan,
        // kun käyttäjä avaa /Books-sivun.
        //
        // Selain lähettää HTTP GET -pyynnön:
        // /Books
        //
        // Controller hakee kirjat tietokannasta
        // ja lähettää ne Index.cshtml-näkymälle.
        //
        // Index.cshtml renderöi _BooksList-partialin.
        //
        // _BooksList.cshtml päättää User.IsInRole("Admin")
        // perusteella:
        //
        // - Admin → taulukko/listanäkymä
        // - Tavallinen käyttäjä → korttinäkymä
        public async Task<IActionResult> Index(int? pageNumber)
        {
            // Määritetään montako kirjaa näytetään yhdellä sivulla.
            //
            // Admin näkee 10 kirjaa per sivu.
            // Tavallinen käyttäjä näkee 8 kirjaa per sivu.
            int pageSize = User.IsInRole("Admin") ? 10 : 8;

            // Rakennetaan tietokantakysely kirjoihin.
            //
            // Include(b => b.Author)
            // hakee mukaan kirjan kirjailijan.
            //
            // Include + ThenInclude
            // hakee mukaan kirjan genret.
            //
            // AsNoTracking()
            // nopeuttaa hakua,
            // koska tietoja vain näytetään eikä muokata.
            var books = db.Books
                .Include(b => b.Author)
                .Include(b => b.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .AsNoTracking();

            // PaginatedList.CreateAsync()
            // hakee tietokannasta vain yhden sivun tiedot,
            // eikä kaikkia kirjoja kerralla.
            //
            // pageNumber ?? 1 tarkoittaa:
            // jos sivunumeroa ei ole annettu,
            // käytetään sivua 1.
            PaginatedList<Book> model = await PaginatedList<Book>.CreateAsync(
                books,
                pageNumber ?? 1,
                pageSize);

            // Lähetetään paginoitu kirjalista
            // Index.cshtml-näkymälle.
            //
            // Index.cshtml renderöi _BooksList-partialin,
            // joka näyttää eri näkymän käyttäjän roolin perusteella.
            return View("Index", model);
        }
        

        // GET: Books/Details/5
        // Näyttää yhden kirjan tarkemmat tiedot.
        // id kertoo mikä kirja haetaan.
        // returnAction kertoo mistä näkymästä käyttäjä tuli.
        public async Task<IActionResult> Details(int? id, string? returnAction)
        {
            // Jos id puuttuu URL:sta, palautetaan 404.
            if (id == null)
                return NotFound();

            // Haetaan kirja id:n perusteella.
            // Mukaan haetaan kirjailija ja genret.
            var book = await db.Books
                .Include(b => b.Author)
                .Include(b => b.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .FirstOrDefaultAsync(m => m.BookId == id);

            // Jos kirjaa ei löydy, palautetaan 404.
            if (book == null)
                return NotFound();

            // Lähetetään returnAction näkymälle Back-painiketta varten.
            ViewBag.ReturnAction = returnAction;

            // Lähetetään yksi Book-olio Details.cshtml-näkymälle.
            return View(book);
        }

        // GET: Books/Create
        // Näyttää uuden kirjan luontilomakkeen.
        // Vain Admin-rooli pääsee tähän.
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            // Luodaan kirjailijoista dropdown-lista.
            // AuthorId on value, FullName näkyy käyttäjälle.
            ViewBag.AuthorId = new SelectList(
                db.Authors.Select(a => new
                {
                    a.AuthorId,
                    FullName = a.Firstname + " " + a.Surname
                }),
                "AuthorId",
                "FullName"
            );

            // Luodaan ViewModel Create-näkymälle.
            // AvailableGenres sisältää kaikki genret checkboxeja varten.
            var viewModel = new BookCreateViewModel
            {
                AvailableGenres = db.Genres
                    .OrderBy(g => g.Name)
                    .ToList()
            };

            // Lähetetään tyhjä lomake Create.cshtml-näkymään.
            return View(viewModel);
        }

        // POST: Books/Create
        // Tämä suoritetaan, kun admin painaa Create-lomakkeen tallennuspainiketta.
        // Lomake lähettää BookCreateViewModelin controllerille.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(BookCreateViewModel viewModel)
        {
            // Tarkistetaan lomakkeen validointi.
            // Jos jokin pakollinen kenttä puuttuu, palataan lomakkeeseen.
            if (!ModelState.IsValid)
            {
                // Dropdown pitää rakentaa uudelleen,
                // koska ViewBag ei säily automaattisesti POST-pyynnön jälkeen.
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

                // Genrelista rakennetaan uudelleen checkboxeja varten.
                viewModel.AvailableGenres = db.Genres
                    .OrderBy(g => g.Name)
                    .ToList();

                // Palautetaan lomake käyttäjälle korjattavaksi.
                return View(viewModel);
            }

            // Käydään läpi käyttäjän valitsemat genret.
            // Jokaisesta valitusta genrestä luodaan BookGenre-liitos.
            foreach (var genreId in viewModel.SelectedGenreIds)
            {
                viewModel.Book.BookGenres.Add(new BookGenre
                {
                    GenreId = genreId
                });
            }

            // Lisätään uusi kirja tietokantakontekstiin.
            db.Books.Add(viewModel.Book);

            // Tallennetaan uusi kirja ja genreliitokset tietokantaan.
            await db.SaveChangesAsync();

            // Tallennuksen jälkeen palataan Books-listaan.
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Edit/5
        // Näyttää kirjan muokkauslomakkeen.
        // Vain Admin saa muokata kirjoja.
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            // Jos id puuttuu, palautetaan 404.
            if (id == null)
                return NotFound();

            // Haetaan muokattava kirja ja sen nykyiset genreliitokset.
            var book = await db.Books
                .Include(b => b.BookGenres)
                .FirstOrDefaultAsync(b => b.BookId == id);

            // Jos kirjaa ei löydy, palautetaan 404.
            if (book == null)
                return NotFound();

            // Luodaan kirjailija-dropdown muokkauslomakkeelle.
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

            // Luodaan ViewModel Edit-näkymälle.
            var viewModel = new BookEditViewModel
            {
                // Muokattavan kirjan tiedot.
                Book = book,

                // Kaikki mahdolliset genret.
                AvailableGenres = db.Genres
                    .OrderBy(g => g.Name)
                    .ToList(),

                // Kirjan nykyiset genre-ID:t,
                // jotta checkboxit voidaan näyttää valmiiksi valittuina.
                SelectedGenreIds = book.BookGenres
                    .Select(bg => bg.GenreId)
                    .ToList()
            };

            // Lähetetään ViewModel Edit.cshtml-näkymälle.
            return View(viewModel);
        }

        // POST: Books/Edit/5
        // Tämä suoritetaan, kun admin painaa Edit-lomakkeen Save-painiketta.
        // Vastaanottaa URL:n id:n ja lomakkeen ViewModelin.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, BookEditViewModel viewModel)
        {
            // Tarkistetaan että URL:n id ja lomakkeen BookId vastaavat toisiaan.
            if (id != viewModel.Book.BookId)
                return NotFound();

            // Jos lomakkeessa on virheitä, palautetaan sama näkymä.
            if (!ModelState.IsValid)
            {
                // Dropdown rakennetaan uudelleen virhetilannetta varten.
                ViewData["AuthorId"] = new SelectList(
                    db.Authors.Select(a => new
                    {
                        a.AuthorId,
                        FullName = a.Firstname + " " + a.Surname
                    }),
                    "AuthorId",
                    "FullName",
                    viewModel.Book.AuthorId
                );

                // Genrelista rakennetaan uudelleen.
                viewModel.AvailableGenres = db.Genres
                    .OrderBy(g => g.Name)
                    .ToList();

                // Palautetaan lomake korjattavaksi.
                return View(viewModel);
            }

            // Haetaan tietokannasta oikea kirja,
            // jota päivitetään lomakkeen tiedoilla.
            var bookFromDb = await db.Books
                .Include(b => b.BookGenres)
                .FirstOrDefaultAsync(b => b.BookId == id);

            // Jos kirjaa ei enää löydy, palautetaan 404.
            if (bookFromDb == null)
                return NotFound();

            // Päivitetään kirjan perustiedot lomakkeen arvoilla.
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

            // Poistetaan vanhat genreliitokset.
            bookFromDb.BookGenres.Clear();

            // Lisätään uudet genreliitokset käyttäjän valintojen mukaan.
            foreach (var genreId in viewModel.SelectedGenreIds)
            {
                bookFromDb.BookGenres.Add(new BookGenre
                {
                    BookId = bookFromDb.BookId,
                    GenreId = genreId
                });
            }

            // Tallennetaan kaikki muutokset tietokantaan.
            await db.SaveChangesAsync();

            // Palataan Books-listaan.
            return RedirectToAction(nameof(Index));
        }

        // GET: Books/Delete/5
        // Näyttää poiston vahvistussivun.
        // Vain Admin voi poistaa kirjoja.
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            // Jos id puuttuu, palautetaan 404.
            if (id == null)
                return NotFound();

            // Haetaan poistettava kirja,
            // sekä siihen liittyvä kirjailija ja genret.
            var book = await db.Books
                .Include(b => b.Author)
                .Include(b => b.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .FirstOrDefaultAsync(m => m.BookId == id);

            // Jos kirjaa ei löydy, palautetaan 404.
            if (book == null)
                return NotFound();

            // Lähetetään kirja Delete.cshtml-vahvistusnäkymälle.
            return View(book);
        }

        // POST: Books/Delete/5
        // Tämä suoritetaan, kun admin vahvistaa poiston Delete-näkymässä.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // Haetaan poistettava kirja id:n perusteella.
            var book = await db.Books.FindAsync(id);

            // Jos kirja löytyy, poistetaan se.
            if (book != null)
            {
                db.Books.Remove(book);
            }

            // Tallennetaan poisto tietokantaan.
            await db.SaveChangesAsync();

            // Palataan Books-listaan.
            return RedirectToAction(nameof(Index));
        }

        // Apumetodi, joka tarkistaa onko kirja olemassa.
        // Tätä voidaan käyttää esimerkiksi virhetilanteiden tarkistamiseen.
        private bool BookExists(int id)
        {
            return db.Books.Any(e => e.BookId == id);
        }

        // GET: Books/Browse
        // Tämä suoritetaan, kun käyttäjä painaa etusivulla Browse Books -painiketta.
        // Näyttää kirjat käyttäjäpuolen korttinäkymänä.
        public async Task<IActionResult> Browse()
        {
            // Haetaan kaikki kirjat.
            // Mukaan haetaan kirjailija ja genret.
            // AsNoTracking nopeuttaa hakua, koska tietoja vain näytetään.
            var books = await db.Books
                .Include(b => b.Author)
                .Include(b => b.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .AsNoTracking()
                .OrderBy(b => b.Title)
                .ToListAsync();

            // Lähetetään kirjalista Browse.cshtml-korttinäkymälle.
            return View(books);
        }

        // GET: Books/Search?query=hakusana&pageNumber=1
        // Tämä suoritetaan, kun käyttäjä hakee kirjoja hakukentällä.
        public async Task<IActionResult> Search(string query, int? pageNumber)
        {
            // Jos hakusana on tyhjä, palataan Books-listaan.
            if (string.IsNullOrWhiteSpace(query))
            {
                return RedirectToAction(nameof(Index));
            }

            // Adminille 10 tulosta sivulle, tavalliselle käyttäjälle 8.
            int pageSize = User.IsInRole("Admin") ? 10 : 8;

            // Rakennetaan hakukysely.
            // Haetaan mukaan kirjailija ja genret.
            // Haku kohdistuu kirjan nimeen ja ISBN-numeroon.
            var books = db.Books
                .Include(b => b.Author)
                .Include(b => b.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .Where(b =>
                    EF.Functions.Like(b.Title, $"%{query}%") ||
                    EF.Functions.Like(b.Isbn, $"{query}%"))
                .AsNoTracking();

            // Tallennetaan hakusana näkymää varten.
            ViewData["CurrentFilter"] = query;

            // Lähetetään hakutulokset takaisin Index.cshtml-näkymälle.
            //
            // Index.cshtml EI itse renderöi kirjojen HTML:ää,
            // vaan kutsuu _BooksList-partial viewtä:
            //
            // @await Html.PartialAsync("_BooksList", Model)
            //
            // Partial View tarkoittaa näkymän osaa,
            // joka voidaan renderöidä erikseen
            //
            // _BooksList.cshtml sisältää logiikan,
            // joka tarkistaa käyttäjän roolin:
            //
            // if (User.IsInRole("Admin"))
            //
            // ja näyttää:
            //
            // - Adminille taulukko/listanäkymän
            // - Tavalliselle käyttäjälle korttinäkymän
            //
            // Controller siis vain hakee datan,
            // mutta Partial View päättää miltä käyttöliittymä näyttää.
            return View("Index", await PaginatedList<Book>.CreateAsync(
                books,
                pageNumber ?? 1,
                pageSize));
        }

        // GET: Books/LiveSearch?query=...
        // Tämä suoritetaan, kun JavaScript hakee kirjoja ilman koko sivun lataamista.
        public async Task<IActionResult> LiveSearch(string? query)
        {
            // Rakennetaan kysely kirjoihin.
            // Mukaan haetaan kirjailijat ja genret.
            var booksQuery = db.Books
                .Include(b => b.Author)
                .Include(b => b.BookGenres)
                    .ThenInclude(bg => bg.Genre)
                .AsQueryable();

            // Jos hakusana ei ole tyhjä,
            // lisätään hakuehto kirjan nimeen ja ISBN-numeroon.
            if (!string.IsNullOrWhiteSpace(query))
            {
                booksQuery = booksQuery.Where(b =>
                    EF.Functions.Like(b.Title, $"%{query}%") ||
                    EF.Functions.Like(b.Isbn, $"%{query}%"));
            }

            // Suoritetaan kysely ja muutetaan tulokset listaksi.
            var results = await booksQuery
                .AsNoTracking()
                .ToListAsync();

            // Palautetaan vain osittainen näkymä,
            // ei koko sivua.
            // JavaScript päivittää tämän avulla kirjalistan.
            return PartialView("_BooksList", results);
        }
    }
}