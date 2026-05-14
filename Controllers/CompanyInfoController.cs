using Microsoft.AspNetCore.Mvc;

namespace PublishingHouseApp.Controllers
{
    //CompanyController vastaa yritysesittelysivusta
    //Kun käyttäjä painaa navigaation "Aoubt Us" linkkiä
    //selai lähettää GET-pyynnön osoitteeseen /Company
    public class CompanyController : Controller
    {

        //GET: /Company
        //Palauttaa Views/Company/Index-näkymän, joka sisältää yrityksen esittelytiedot
        public IActionResult Index()
        {
            return View();
        }
    }
}
