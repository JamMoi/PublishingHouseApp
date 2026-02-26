using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
//Rooli MVC-sovelluksessa:
//Tämä model toimii siltana Viewin (lomake) ja Controllerin välillä.
//Kun käyttäjä täyttää lomakkeen ja lähettää sen, tiedot siirtyvät ContactControlleriin ContactFormModel-objektina.
//Controller voi tarkistaa tiedot, tallentaa ne tietokantaan, lähettää sähköpostin tai käsitellä liitetiedoston.
namespace PublishingHouseApp.Models
{
    public class ContactFormModel
    {
        [Required]
        [Display(Name = "Full Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]//varmistaa on nko email kenttää kirjoitettu arvo kelvollinen sähköpostiosoite
        [Display(Name = "Email Address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Message")]
        public string Message { get; set; }

        [Display(Name = "Attach your script")]
        public IFormFile? File { get; set; }//Mahdollistaa tiedoston liittämisen lomakkeeseen. IFormFile on asp.net coren tarjoama tiedostotyyppi.
    }
}
