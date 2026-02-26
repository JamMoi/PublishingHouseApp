using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PublishingHouseApp.Models;
using Microsoft.EntityFrameworkCore; // Entity Framework Core -k‰skyjen k‰yttˆ‰ varten, kuten ToListAsync, Include( jen) navigointiominaisuuksien lataamiseen.
using PublishingHouseApp.Data;       // DbContext sijaitsee t‰‰ll‰

namespace PublishingHouseApp.Controllers // t‰m‰  on oletus controlleri
{
    public class HomeController : Controller
    {
        private readonly PublishingHouseContext context; // DbContext-luokan olio, joka tarjoaa p‰‰syn tietokantaan. siis t‰m‰n kautta haetaan data.
        private readonly ILogger<HomeController> logger;
        public HomeController(PublishingHouseContext context, ILogger<HomeController> logger) //Dependency Injection - ASP.NET Core antaa kontekstin automaattisestikonstruktori, joka ottaa DbContext-olion parametrina ja tallentaa sen luokan kentt‰‰n.
        {
            this.context = context;
            this.logger = logger;
        }
    
        public async Task<IActionResult> Index() // T‰m‰ hakee kirjat carouselia varten

        {
            // Haetaan vain saatavilla olevat kirjat
            var latestBooks = await context.Books
            .Where(b => b.IsAvailable)// N‰ytet‰‰n vain myynniss‰ olevat
            .OrderByDescending(b => b.PublishedDate)//Uusimmat ensin
            .Take(5)//otetaan vain 5 uusinta kirjaa
            .ToListAsync(); //Hakee kaikki kirjat tietokannasta ja lataa niihin liittyv‰t Author-tiedot (Include) tehokkaasti yhdell‰ kyselyll‰.

            return View(latestBooks);   //L‰hett‰‰ haetut kirjat n‰kym‰‰n Index-sivulle. N‰kym‰ odottaa List<Book>-mallia, joka sis‰lt‰‰ uusimmat saatavilla olevat kirjat.
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0,
                       Location = ResponseCacheLocation.None,
                       NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}