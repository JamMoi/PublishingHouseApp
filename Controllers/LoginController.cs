using Microsoft.AspNetCore.Identity; // Tuo käyttöön ASP.NET Core Identity -kirjaston.Sen avulla voidaan hallita käyttäjiä, salasanoja, kirjautumista ja uloskirjautumista.Tästä tulee käyttöön esimerkiksi UserManager ja SignInManager.
using Microsoft.AspNetCore.Mvc;//Tuo käyttöön MVC-toiminnot.Tarvitaan, jotta controller voi palauttaa näkymiä (View) ja uudelleenohjauksia (RedirectToAction).
using Microsoft.DotNet.Scaffolding.Shared.CodeModifier.CodeChange;
using PublishingHouseApp.Models;// Tuo käyttöön projektin omat mallit.controller käyttää mm: ApplicationUserRegisterViewModelLnViewModel

namespace PublishingHouseApp.Controllers // Määrittää, että tämä luokka kuuluu Controllers-kansioon / nimeämisalueeseen.MVC löytää controllerit tästä namespace-alueesta.
{
    public class AccountController : Controller //luodaan Account controlleri. vastaa käyttäjätunnuksii liittyvistä toiminnoista: rekisteröityminen, kirjautuminen, uloskirjaituminen.
    {
        private readonly UserManager<ApplicationUser> _userManager;//tallentaa olion jonka avulla voidaan hallita käyttäjiä tietokannassa; _userManager osaa esim. luoda käyttäjän, etsiä käyttän, vaihtAA salasanan, tarkistaa käyttäjän tiedot
        private readonly SignInManager<ApplicationUser> _signInManager; // Tallentaa olion, joka vastaa kirjautumisesta ja uloskirjautumisesta. _signInManager osaa esim, kirjata käyttäjän sisäänkirjata käyttäjän ulos, tarkistaa salasanan

        //Tämä on controllerin konstruktori. ASP.NET Core lähettää tänne automaattisesti(Dependency Injection) valmiit UserManager-ja SignInManager-oliot.
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager; //Tallentaa konstruktorilta saadut oliot controllerin omiin muuttujiin. voidaan käyttää myöhemmin kaikissa metodeissa.
            _signInManager = signInManager;
        }

        [HttpGet] //Kertoo, että seuraava metodi reagoi HTTP GET -pyyntöön.
        //GET tarkoittaa yleensä sivun avaamista selaimessa
        public IActionResult Register()//ämä metodi suoritetaan, kun käyttäjä menee osoitteeseen:/Account/Register
        {
            return View();//Palauttaa rekisteröintisivun näkymän
            //MVC etsii tiedoston:Views/Account/Register.cshtml. Tähän näkymään käyttäjä kirjoittaa sähköpostin ja salasanan.
        }

        [HttpPost] //Kertoo, että seuraava metodi reagoi lomakkeen lähettämiseen. POST tarkoittaa, että käyttäjä lähettää tietoao palvelimelle.
        public async Task<IActionResult> Register(RegisterViewModel model)//Tämä metodi suoritetaan, kun rekisteröintilomake lähetetään.MVC ottaa lomakkeelta tiedot ja sijoittaa ne RegisterViewModel-olioon.esim:model.Email,model.Password
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new ApplicationUser//Luodaan uusi käyttäjäolio
            {
                UserName = model.Email,//Käyttäjän käyttäjänimeksi asetetaan sähköposti.ApplicationUser lähetetään seuraavaksi UserManagerille.
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);//Yrittää luoda käyttäjän tietokantaan
                                                                              //Lähettää:käyttäjäolion(user)salasanan(model.Password)Identity:tallentaa käyttäjän,hashää salasanan
            if (result.Succeeded)//Tarkistaa onnistuiko käyttäjän luonti
            {
                await _signInManager.SignInAsync(user, isPersistent: false); //Kirjaa käyttäjän sisään heti rekisteröitymisen jälkeen.isPersistent: false tarkoittaa:kirjautuminen säilyy vain selaimen sulkemiseen asti"Muista minut" ei ole käytössä tässä
                return RedirectToAction("Index", "Home");//Kun käyttäjä on kirjautunut sisään, selain ohjataan etusivulle: /Home/Index
            }

            foreach (var error in result.Errors) //Jos käyttäjän luonti epäonnistui, käydään kaikki virheet läpi.kuten liian lyuhyt salasana tai sposti jo käytössä
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model); //Palauttaa rekisteröintisivun uudelleen virheilmoitusten kanssa.
        }

        [HttpGet]
        public IActionResult Login()//Tämä suoritetaan, kun käyttäjä avaa kirjautumissivun:
        {
            return View();//Palauttaa näkymän:Views/Account/Login.cshtml
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)//Tämä suoritetaan, kun käyttäjä painaa "Kirjaudu sisään" -painiketta.Lomakkeen tiedot sijoitetaan LoginViewModel-olioon.
        {
            if (!ModelState.IsValid)//Jos käyttäjä ei täyttänyt kaikkia kenttiä oikein:
                return View(model);//avaa krijautumissiuvn uudelleen ja näyttää vriheet

            var result = await _signInManager.PasswordSignInAsync(//Yrittää kirjata käyttäjän sisään.Lähettää:sähköpostin salasananRememberMe - valinnan RememberMe vaikuttaa siihen, pysyykö käyttäjä kirjautuneena selaimen sulkemisen jälkeen.lockoutOnFailure: false tarkoittaa:käyttäjää ei lukita vaikka salasana olisi monta kertaa väärin
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)//Jos kirjautuminen onnistui:
            {
                return RedirectToAction("Index", "Home");//Käyttäjä ohjataan etusivulle.
            }

            ModelState.AddModelError("", "Virheellinen sähköposti tai salasana."); //Jos kirjautuminen epäonnistui: lisätään virheilmoitus näkymään
            return View(model);//Näyttää kirjautumissivun uudelleen.
        }

        [HttpPost]
        public async Task<IActionResult> Logout()//Tämä metodi suoritetaan, kun käyttäjä painaa "Kirjaudu ulos". Se vastaanottaa POST-pyynnön esimerkiksi napista.
        {
            await _signInManager.SignOutAsync();//Poistaa käyttäjän kirjautumisen.Käyttäjän kirjautumiscookie poistetaan.
            return RedirectToAction("Index", "Home");//Uloskirjautumisen jälkeen käyttäjä ohjataan takaisin etusivulle.
        }
    }
}