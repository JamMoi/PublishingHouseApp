using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PublishingHouseApp.Models;
using Microsoft.EntityFrameworkCore; // Entity Framework Core -käskyjen käyttöä varten, kuten ToListAsync ja Include
using PublishingHouseApp.Data;       // DbContext sijaitsee täällä

namespace PublishingHouseApp.Controllers // tämä on oletuscontrolleri
{
    public class HomeController : Controller
    {
        private readonly PublishingHouseContext context; // DbContext-luokan olio, jonka kautta haetaan data tietokannasta
        private readonly ILogger<HomeController> logger;

        public HomeController(PublishingHouseContext context, ILogger<HomeController> logger) //Dependency Injection
        {
            this.context = context;
            this.logger = logger;
        }

        public async Task<IActionResult> Index() // Tämä hakee kirjat carouselia varten
        {
            // Haetaan vain saatavilla olevat kirjat
            var latestBooks = await context.Books
                .Include(b => b.Author) //Ladataan mukaan myös kirjailijan tiedot
                .Where(b => b.IsAvailable) //Näytetään vain myynnissä olevat kirjat
                .OrderByDescending(b => b.PublishedDate) //Uusimmat ensin
                .Take(5) //Otetaan vain 5 uusinta kirjaa
                .ToListAsync(); //Suoritetaan kysely tietokantaan

            return View(latestBooks); //Lähettää haetut kirjat näkymään Index-sivulle
        }

        // GET: /Home/Privacy EI KÄYTÖSSÄ
        public IActionResult Privacy() //public = metodi on julkinen, eli sitä voidaan kutsua luokan ulkopuolelta
                                       //IActionResult = metodin palautustyyppi. Se kertoo, että metodi palauttaa jonkin MVC-toiminnon tuloksen, esimerkiksi näkymän (View).
        {                               //Privacy() = metodin nimi. Tämä metodi käsittelee yleensä /Privacy-sivun.
            return View();              //palauttaa näkymän, joka sijaitsee Views/Home/Privacy.cshtml-tiedostossa. Tämä on oletusnäkymä, joka luodaan, kun luodaan uusi ASP.NET Core MVC -projekti.
        }

        [ResponseCache(Duration = 0, //ResponseCache-attribuutti määrittää, että tämän metodin tuottamaa sisältöä ei saa välimuistittaa. Tämä on tärkeää virhesivujen kohdalla, jotta käyttäjät näkevät aina ajantasaisen virhesivun.
                       Location = ResponseCacheLocation.None, //Location = ResponseCacheLocation.None tarkoittaa, että sisältöä ei saa tallentaa mihinkään välimuistiin, kuten selaimen välimuistiin tai välityspalvelimen välimuistiin.
                       NoStore = true)]//NoStore = true -> sisältöä ei saa tallentaa mihinkään välimuistiin, eikä sitä saa käyttää uudelleen myöhemmin. Tämä on erityisen tärkeää virhesivujen kohdalla, jotta käyttäjät näkevät aina ajantasaisen virhesivun.
        public IActionResult Error() //public = metodi on julkinen, eli sitä voidaan kutsua luokan ulkopuolelta
                                     //IActionResult = metodin palautustyyppi. Se kertoo, että metodi palauttaa jonkin MVC-toiminnon tuloksen, esimerkiksi näkymän (View).
        {                               //Error() = metodin nimi. Tämä metodi käsittelee yleensä virhesivun näyttämisen.
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}