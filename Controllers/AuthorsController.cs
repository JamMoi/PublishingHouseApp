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
    public class AuthorsController : Controller
    {
        private readonly PublishingHouseContext db; //tietokantayhteys, Entity Framework -konteksti

        public AuthorsController(PublishingHouseContext context)
        {
            db = context;
        }

        // GET: Authors

        //hakee kaikki kirjailijat tietokannasta ja näyttää ne listana
        public async Task<IActionResult> Index()
        {
            return View(await db.Authors.ToListAsync());
        }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int? id) //näyttää yksittäisen kirjailijan tiedot. Jos id puuttuu tai kirjailijaa ei löydy -> virhe. Muuten palauttaa näkymän.
        {
            if (id == null)
            {
                return NotFound();//jos id puuttuu, palauttaa virheen
            }

            var author = await db.Authors
                .FirstOrDefaultAsync(m => m.AuthorId == id);
            if (author == null)
            {
                return NotFound();
            }

            return View(author);
        }

        // GET: Authors/Create
        public IActionResult Create() //näyttää lomakkeen uuden kirjailijan lisäämiseksi
        {
            return View();
        }

        // POST: Authors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AuthorId,Firstname,Surname,Email,PhoneNumber,Bio,Date")] Author author) //kun lomake lähetetään, tarkistaa tiedot (ModelState.IsValid) ja lisää uuden kirjailijan tietokantaan. Tallentaa muutokset. Palaa listauskeen.
        {                                                                                                                       //[Bind] rajoittaa, mitä kenttiä sallitaan lomakkeelta      
            if (ModelState.IsValid)
            {
                db.Add(author);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            return View(author);
        }

        // GET: Authors/Edit/5
        public async Task<IActionResult> Edit(int? id)//hakee muokattavan kirjailijan id:n perusteella
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
        public async Task<IActionResult> Edit(int id, [Bind("AuthorId,Firstname,Surname,Email,PhoneNumber,Bio,Date")] Author author)//kun muokkaus lähetetään, tarkistaa että URL:n id ja lomakkeen id täsmäävät. Päivittää kirjailijan. tallentaa tietokantaan. käsittelee mahdollisen samanaikaisuusvirheen (DbUpdateConcurrencyException).
        {
            if (id != author.AuthorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(author);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorExists(author.AuthorId))
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
            return View(author);
        }

        // GET: Authors/Delete/5
        public async Task<IActionResult> Delete(int? id) //Näyttää poistovahvistusnäkymän.
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

        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Haetaan kirjailija ja hänen kirjat tietokannasta (include)
            var author = await db.Authors
                .Include(a => a.Books) //lataaa kirjailijan sekä hänen kirjansa, varmistetaan että krijalista ladataan
                .FirstOrDefaultAsync(a => a.AuthorId == id);
            if (author == null)
            {
                return NotFound();
            }

            //tarkastetaan , onko kirjailijalla kirjoja, Jos kirjailijalla on kirjoja estetään poisto
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

            return RedirectToAction(nameof(Index));
        }
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var author = await db.Authors.FindAsync(id);
        //    if (author != null)
        //    {
        //        db.Authors.Remove(author);
        //    }

        //    await db.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        private bool AuthorExists(int id)//Apumetodi->Tarkistaa onko kirjailija olemassa. Käytetään concurrency-virheen käsittelyssä.
        {
            return db.Authors.Any(e => e.AuthorId == id);
        }
    }
}

