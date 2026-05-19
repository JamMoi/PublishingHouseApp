using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using PublishingHouseApp.Models;
using System.ComponentModel.DataAnnotations;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;

namespace PublishingHouseApp.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index() // Vastaa GET pyyntöön, näyttää yhteystiedot näkymän
        {
            return View();
        }

        
        
    }
}
