using Microsoft.AspNetCore.Mvc; //Tarjoaa MVC:n Controller- ja ActionResult-luokat
using Microsoft.AspNetCore.Mvc.Rendering;//Auttaa luomaan HTML-elementtejä Razorissa (esim. <select> listoja).
using Microsoft.EntityFrameworkCore;//Tarjoaa Entity Framework Core -toiminnallisuudet tietokantakyselyihin ja -muutoksiin.
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using PublishingHouseApp.Data;//tietokantayhteys, DbContext-luokka
using PublishingHouseApp.Models;//Sovelluksen tietomallit, kuten Author ja Book
using System;//Perus .NET -toiminnallisuudet(Listat, päivämäärät, LINQ, jne.)
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public async Task<IActionResult> Index() //// Index()-metodi hakee kaikki kirjailijat tietokannasta ja Palauttaa listan Index.cshtml-Viewille. 
        {
            var authors = await db.Authors.Include(a => a.Books).ToListAsync(); //ladataan myös kirjailijoiden kirjat
            return View(authors);                                               //Hakee kaikki Author-tietueet tietokannasta. Lähettää ne Index.cshtml-viewille. View ottaa vastaan IEnumerable<Author> ja näyttää taulukon.
                                                                                //kutsujetku: Browser → / Authors → Controller.Index() → Index.cshtml

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
        public async Task<IActionResult> Create([Bind("AuthorId,Firstname,Surname,Email,PhoneNumber,Bio")] Author author)// KÄSITTELEE TALLENNUKSEN. kun lomake lähetetään, tarkistaa tiedot (ModelState.IsValid) ja lisää uuden kirjailijan tietokantaan. Tallentaa muutokset. Palaa listauskeen.
        //vastaanottaa lomakkeesta Author-olion, Asettaa automaattiseti piävämäärän, Tallentaa Authorin tietokantaan, Ohjaa takaisin indeksiin.
        //Kutsuketju: Create.cshtml(form POST) → Controller.Create(POST) → Save → Redirect(Index) ///[Bind] rajoittaa, mitä kenttiä sallitaan lomakkeelta
        {
            if (ModelState.IsValid)
            {
                author.Date = DateTime.Today; //asettaa päivämäärän nykyhetkeen
                db.Add(author);              //lisää uuden author-olion tietokantakontekstiin
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(author);
        }

        // GET: Authors/Edit/5 
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
            if (author.Books.Any())
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
    }
}

