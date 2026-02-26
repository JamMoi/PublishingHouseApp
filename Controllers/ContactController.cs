using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using PublishingHouseApp.Models;
using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace PublishingHouseApp.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index() // Vastaa GET pyyntöön, näyttää lomakkeen 
        {
            return View();
        }

        // Additional actions for handling contact form submissions can be added here
        //POST: /Contact/Submit
        //ämä metodi käsittelee lomakkeen lähetyksen. ContactFormModel model vastaanottaa tiedot lomakkeelta(Name, Email, Message, File).ModelState.IsValid tarkistaa, että kaikki pakolliset kentät ja validoinnit([Required], [EmailAddress]) ovat kunnossa.
//Jos lomake on validi:Voit käsitellä tiedot: lähettää sähköpostin, tallentaa tietokantaan tms.
//Tässä tapauksessa vain asetetaan onnistumisviesti TempData["SuccessMessage"] ja uudelleenohjataan samaan lomakkeeseen.
//Jos lomake ei ole validi: Lomake näytetään uudelleen käyttäjälle, ja virheilmoitukset näkyvät asp-validation-for-tageissa Index.cshtml:ssa.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(ContactFormModel model)
        {
            if (ModelState.IsValid)
            {
                // Process the form data (e.g., send email, save to database)
                // For now, just redirect to a thank you page
                TempData["SuccessMessage"] = "Thank you for contacting us! We will contact you as soon as possibel";
                return RedirectToAction("Index");
            }
            // If the model state is invalid, return to the form with validation messages
            return View(model);
        }
    }
}
