using Microsoft.AspNetCore.Mvc; //Tarjoaa MVC:n Controller- ja ActionResult-luokat
using Microsoft.AspNetCore.Mvc.Rendering;//Auttaa luomaan HTML-elementtejä Razorissa (esim. <select> listoja).
using Microsoft.EntityFrameworkCore;//Tarjoaa Entity Framework Core -toiminnallisuudet tietokantakyselyihin ja -muutoksiin.
using PublishingHouseApp.Data;//tietokantayhteys, DbContext-luokka
using PublishingHouseApp.Models;//Sovelluksen tietomallit, kuten Author ja Book
using System;//Perus .NET -toiminnallisuudet(Listat, päivämäärät, LINQ, jne.)
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization; //Mahdollistaa roolipohjaisen pääsynhallinnan, jos halutaan rajoittaa tiettyihin toimintoihin vain tietyt käyttäjät (esim. Adminit).


namespace PublishingHouseApp.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly PublishingHouseContext db; //tietokantayhteys, Entity Framework -konteksti.
        //db on tietokantayhteys, jota käytetään Author- ja Book-tietojen hakemiseen ja tallentamiseen.

        public AuthorsController(PublishingHouseContext context)//PublishingHouseContext injektoidaan automaattisesti (dependency injection).
        {
            db = context; //Tallennetaan db-muuttujaan, jota controllerin metodit käyttävät.
        }

        // GET: Authors

        public async Task<IActionResult> Index(int? pageNumber) { 
        
            // Määritellään montako itemiä näytetään per sivu
            // Adminille enemmän, käyttäjälle vähemmän
            int pageSize = User.IsInRole("Admin") ? 10 : 8;

            // Rakennetaan tietokantakysely (ei vielä suoriteta!)
            // Include = ladataan myös kirjailijan kirjat mukaan
            // AsNoTracking = nopeampi, koska dataa ei muokata
            var authors = db.Authors
                .Include(a => a.Books)
                .AsNoTracking();

            // Luodaan paginoitu lista:
            // pageNumber ?? 1 → jos pageNumber on null, käytetään sivua 1
            return View(await PaginatedList<Author>.CreateAsync(
                authors,
                pageNumber ?? 1,
                pageSize));
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id) // details(int? id) tarkistaa, onko id annettu),hakee yksittäisen kirjailijan tiedot, ja lähettääs en Details.cshtml-sivulle. Jos id puuttuu tai kirjailijaa ei löydy -> virhe. Muuten palauttaa näkymän.
        //Kutsuketju: linkki listasta --> details --> controlelri --> details.cshtml.
        {
            if (id == null)
            {
                return NotFound();//jos id puuttuu, palauttaa virheen
            }

            var author = await db.Authors
                .Include(a => a.Books) //ladataan myös kirjailijan kirjat
                .FirstOrDefaultAsync(m => m.AuthorId == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // GET: Authors/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create() // näyttää tyhjän lomakkeen uuden kirjailijan lisäämiseksi.
        //Kutsuketju: Index.cshtml → Create-linkki → Controller.Create() → Create.cshtml
        {
            return View();
        }

        // POST: Authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("AuthorId,Firstname,Surname,Email,PhoneNumber,Bio")] Author author)// KÄSITTELEE TALLENNUKSEN. kun lomake lähetetään, tarkistaa tiedot (ModelState.IsValid) ja lisää uuden kirjailijan tietokantaan. Tallentaa muutokset. Palaa listauskeen.
        //vastaanottaa lomakkeesta Author-olion, Asettaa automaattiseti piävämäärän, Tallentaa Authorin tietokantaan, Ohjaa takaisin indeksiin.
        //Kutsuketju: Create.cshtml(form POST) → Controller.Create(POST) → Save → Redirect(Index) ///[Bind] rajoittaa, mitä kenttiä sallitaan lomakkeelta
        {
            if (ModelState.IsValid)
            {
                author.Date = DateTime.Today; //asettaa päivämäärän nykyhetkeen
                db.Authors.Add(author);              //lisää uuden author-olion tietokantakontekstiin
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(author);
        }

        // GET: Authors/Edit/5 
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)//hakee muokattavan kirjailijan id:n perusteella
        //Hakee muokattavan Authorin. Lähettää sen Edit.cshtml-sivulle.
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await db.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }
            return View(author);
        }

        // POST: Authors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        //TÄMÄ EDIT(POST) METODI KÄSITTELEE EDIT.CSHTML-LOMAKKEEN "SAVE" (VIEW) painikkeen PAINALLUKSEN.
        public async Task<IActionResult> Edit(int id, [Bind("AuthorId,Firstname,Surname,Email,PhoneNumber,Bio,Date")] Author author)//kun muokkaus lähetetään, tarkistaa että URL:n id ja lomakkeen id täsmäävät. Päivittää kirjailijan. tallentaa tietokantaan. käsittelee mahdollisen samanaikaisuusvirheen (DbUpdateConcurrencyException).
        //Tarkistaa URL:ssa olevan ID:n ja lomakkeessa olevan ID:n. ModelStatessa tarkistaa että pakoslliset kentät on näytetty, tietotyypit oikein ja dataannotations säännöt täyttyvät. Päivittää Authorin.Tallentaa tietokantaan.Ohjaa Indexiin.
        {
            if (id != author.AuthorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(author);      //Entity Framework merkitsee author-olion muokatuksi
                    await db.SaveChangesAsync();//tallentaa muutokset tietokantaan
                }
                catch (DbUpdateConcurrencyException) //käsittelee samanaikaisuusvirheen esi. jos toinen käyttäjä on muokannut samaa kirjailijaa
                {
                    if (!AuthorExists(author.AuthorId)) //tarkistaa onko kirjailija edelleen olemassa
                    {
                        return NotFound(); //jos ei ole, palauttaa NotFound
                    }
                    else //muussa tapauksessa heittää virheen uudelleen (KEHITYSVAIHEESSA!?)
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); //Onnistuneen tallennuksen jälkeen ohjaa takaisin Indexiin, jossa näkyy päivitetty lista.
            }
            return View(author);
        }

        // GET: Authors/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id) //hakee authorin ja  Näyttää poistovahvistusnäkymän.
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await db.Authors
                .FirstOrDefaultAsync(m => m.AuthorId == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // POST: Authors/Delete/5
        [HttpPost, ActionName("Delete")]//// Poistetaan kirjailija, jos hänellä EI ole kirjoja
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        //TÄMÄ DELETE(POST) METODI KÄSITTELEE DELETE.CSHTML-LOMAKKEEN (VIEW) "DELETE" NAPIN PAINALLUKSEN.

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Haetaan kirjailija ja hänen kirjat tietokannasta (include)
            var author = await db.Authors
                .Include(a => a.Books) //hakee kirjailijan sekä hänen kirjansa, varmistetaan että krijalista ladataan
                .FirstOrDefaultAsync(a => a.AuthorId == id);
            if (author == null)
            {
                return NotFound();
            }

            //tarkastetaan , onko kirjailijalla kirjoja, Jos kirjailijalla on kirjoja, estetään poisto
            if (author.Books != null && author.Books.Any())
            {
                //Lisätään virheilmoitus ModelStateen
                ModelState.AddModelError("", "Author has books saved, you cannot remove this author");

                //Palauttaa delete näkymän virheilmoituksen kanssa
                return View(author);
            }

            //poistetaan kirjailija, jos hänellä ei ole kirjoja tietokannassa
            db.Authors.Remove(author);
            await db.SaveChangesAsync();

            return RedirectToAction(nameof(Index));//ohjaa takaisin Indexiin
        }

        private bool AuthorExists(int id)//Apumetodi->Tarkistaa onko kirjailija olemassa. Käytetään concurrency-virheen käsittelyssä.
        {
            return db.Authors.Any(e => e.AuthorId == id);
        }

        // GET: Authors/Browse
        // Tämä action suoritetaan, kun käyttäjä painaa
        // Home/Index.cshtml-sivulla "Meet Authors" -painiketta
        // Painike:
        //
        // <a asp-controller="Authors" asp-action="Browse">
        //
        // lähettää selaimesta HTTP GET -pyynnön osoitteeseen:
        // /Authors/Browse
        //
        // Tämä action hakee kaikki kirjailijat tietokannasta,
        // hakee samalla mukaan kirjailijoihin liittyvät kirjat,
        // järjestää kirjailijat sukunimen mukaan aakkosjärjestykseen
        // ja lähettää listan Browse.cshtml-näkymälle.
        //
        // AsNoTracking() tekee hausta kevyemmän,
        // koska tietoja vain näytetään eikä muokata
        public async Task<IActionResult> Browse()
        {
            var authors = await db.Authors
                .Include(a => a.Books) // ladataan mukaan kirjailijan kirjat
                .AsNoTracking() // nopeampi haku vain näyttämistä varten
                .OrderBy(a => a.Surname) //järjestetään sukunimen mukaan aakkosjärjestykseen
                .ToListAsync(); // suorittaa tietokantakyselyn

            return View(authors);   // Lähettää authors-listan Browse.cshtml-näkymälle

        }

        // GET: Authors/Search?query=some+search+term
        // Hakee kirjailijoita nimen perusteella.
        // Tämä toiminto on sallittu kaikille käyttäjille.
        // Hakukenttä lähettää query-parametrin URL:iin:
        // /Authors/Search?query=tolkien
        //
        // EF.Functions.Like toimii SQL LIKE -hakuna tietokannassa.
        //
        // Lopuksi hakutulokset lähetetään takaisin
        // samaan Index.cshtml-näkymään.
        public async Task<IActionResult> Search(string query)
{
    if (string.IsNullOrWhiteSpace(query))  // Tarkistetaan onko hakukenttä tyhjä, null
                                           // tai sisältää vain välilyöntejä.
            {
                return RedirectToAction(nameof(Index)); 
    }
            // Rakennetaan tietokantakysely,
            // joka hakee kirjailijoita nimen perusteella
            var results = await db.Authors
        // Include lataa mukaan myös kirjailijan kirjat,
        // jotta niitä voidaan näyttää näkymässä.
        .Include(a => a.Books)

        .Where(a =>  // Where määrittää hakuehdot
                     // EF.Functions.Like tekee SQL LIKE -haun tietokannassa.
                     // % tarkoittaa "sisältää".
                     //
                     
            EF.Functions.Like(a.Firstname, $"%{query}%") ||
            // Hakee sukunimestä
            EF.Functions.Like(a.Surname, $"%{query}%") ||
            // Hakee koko nimestä
            // esim. "J.R.R Tolkien"
            EF.Functions.Like(a.Firstname + " " + a.Surname, $"%{query}%")
        )
        // AsNoTracking tekee hausta kevyemmän,
        // koska tietoja vain näytetään eikä muokata.
        .AsNoTracking()
        // Suorittaa tietokantakyselyn
        // ja muuttaa tulokset listaksi.
        .ToListAsync();

            // Lähettää hakutulokset takaisin Index.cshtml-näkymälle.
            //
            // Näkymä saa results-listan modelina
            // ja näyttää vain hakua vastaavat kirjailijat
            return View("Index", results);
}

        // GET: Authors/LiveSearch?query=...
        // Tekee dynaamisen haun ja palauttaa vain osittaisen näkymän AJAX-kutsua varten.
        // Tämä toiminto on sallittu kaikille käyttäjille.
        // JavaScript lähettää AJAX-pyynnön ilman sivun uudelleenlatausta.
        //
        // PartialView palauttaa vain taulukon HTML-osan,
        // ei koko sivua.
        // query-parametri vastaanottaa käyttäjän kirjoittaman hakusanan.
        public async Task<IActionResult >LiveSearch (string? query)
        {
            // Rakennetaan tietokantakysely Authors-tauluun.
            //
            // Include(a => a.Books)
            // hakee mukaan myös kirjailijan kirjat,
            // jotta niitä voidaan näyttää näkymässä.
            //
            // AsQueryable() mahdollistaa kyselyn muokkaamisen myöhemmin,
            // esimerkiksi lisäämällä Where-ehtoja.
            var authorQuery = db.Authors
                .Include(a => a.Books)
                .AsQueryable();

            // Tarkistetaan että hakusana ei ole tyhjä,
            // null tai pelkkiä välilyöntejä
            if (!string.IsNullOrWhiteSpace(query))
            {
                // Lisätään tietokantakyselyyn hakuehdot.
                //
                // Contains(query)
                // hakee kaikki kirjailijat,
                // joiden etu- tai sukunimi sisältää hakusanan.
                //
                // Lisäksi haetaan koko nimellä:
                // "Firstname + Surname"
                authorQuery = authorQuery.Where(a =>
                    a.Firstname.Contains(query) ||
                    a.Surname.Contains(query) ||
                    (a.Firstname + " " + a.Surname).Contains(query));
            }
            // Suoritetaan tietokantakysely
            // ja muutetaan tulokset listaksi.
            var results = await authorQuery.ToListAsync();

            // PartialView palauttaa vain yhden osan HTML-sivusta,
            // ei koko näkymää.
            //
            // "_AuthorsTable" sisältää vain päivitettävän taulukon.
            //
            // JavaScript vastaanottaa tämän HTML:n
            // ja päivittää sivun sisällön ilman reloadia.
            return PartialView("_AuthorsTable", results); //palauttaa vain osan näkymästä, joka päivitetään AJAX-kutsulla
        }
    }
}

