using Microsoft.EntityFrameworkCore; //EF Core -kirjasto tietokantayhteyksi‰ varten
using Microsoft.Extensions.Options; //ei ehk‰ tarvita t‰ss‰, mutta usein hyˆdyllinen
using PublishingHouseApp.Data; //Jotta voidaan k‰yt‰‰ omaa tietokantakontekstia PublishingHouseContext
using PublishingHouseApp.Models; //Jotta voidaan k‰ytt‰‰ tietokantamalleja, kuten Book, Author jne.
using Microsoft.AspNetCore.Identity;


//yll‰ tuodaan tarvittavat kirjastot projektiin


var builder = WebApplication.CreateBuilder(args); //t‰ss‰ luodaan olio, johon lis‰t‰‰n palvelut (kuten tietokanta, MVC.jne) joka rakentaa web-sovelluksen


// lis‰t‰‰n palvelut sovellukseen
builder.Services.AddControllersWithViews(); //rekisterˆid‰‰n MVC-palvelut, jotta voidaan k‰ytt‰‰ ohjaimia ja n‰kymi‰

// Lis‰t‰‰n tietokantapalvelu ja m‰‰ritet‰‰n SQL Server -yhteys stringist‰ appsettings.json
builder.Services.AddDbContext<PublishingHouseContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); 

// Idnentity Frameworkin ja ASP.NET Identityn konfigurointi
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true; // Salasanassa on oltava numero
    options.Password.RequireLowercase = true;// Pienikirjain pakollinen
    options.Password.RequireUppercase = false; // Ei pakollista isoja kirjaimia
    options.Password.RequireNonAlphanumeric = false;// Erikoismerkki‰ ei vaadita
    options.Password.RequiredLength = 6; // Minimipituus 6 merkki‰

    options.SignIn.RequireConfirmedAccount = false; // Ei vaadi vahvistettua tili‰ kirjautumiseen
})
.AddEntityFrameworkStores<PublishingHouseContext>()// K‰ytet‰‰n tietokantaa k‰ytt‰jien ja roolien tallentamiseen
.AddDefaultTokenProviders(); // Tarjoaa valmiit tokenit esimerkiksi salasanojen palautukseen

builder.Services.AddRazorPages(); // Mahdollistaa Razor Pages -n‰kym‰t


var app = builder.Build(); //Rakennetaan varsinainen sovellus k‰ytt‰en edell‰ lis‰ttyj‰ palveluita

using (var scope = app.Services.CreateScope())// Luo scope, jotta voidaan k‰ytt‰‰ palveluita alustuksessa

{
    var services = scope.ServiceProvider;

   
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>(); // Haetaan roolien hallinta
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>(); // Haetaan k‰ytt‰jien hallinta

    // Luodaan perusroolit
    string[] roles = { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role)) // Jos roolia ei ole viel‰ olemassa
        {
            await roleManager.CreateAsync(new IdentityRole(role));// Luodaan rooli
        }
    }

    // Lis‰t‰‰n admin-k‰ytt‰j‰ Admin-rooliin
    var adminEmail = "admin@test.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser != null)
    {
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");// Lis‰t‰‰n admin-rooli
        }
    }
}
// Alustetaan dummy-data tietokantaan
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<PublishingHouseContext>();
    DbInitializer.Initialize(context);// T‰ytt‰‰ tietokannan esimerkkidatalla

}
    if (!app.Environment.IsDevelopment())// Virheenk‰sittely tuotantoymp‰ristˆss‰

{
    app.UseExceptionHandler("/Home/Error"); // virheiden korjaus tuotannossa; ohjee virheet error sivulle
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts(); //k‰ytet‰‰n HTTP Strict Transport Security‰, joka pakottaa selaimet k‰ytt‰m‰‰n HTTPS:‰‰
}

app.UseHttpsRedirection();
app.UseStaticFiles(); //kartoitus staattisille resursseille kuten kuville, CSS:lle ja JavaScriptille
app.UseRouting();//Ottaa k‰yttˆˆn reititysj‰rjestelm‰n (eli miten URLit ohjataan controllereille).

app.UseAuthentication();
app.UseAuthorization(); // Aktivoi mahdollisen autorisoinnin (vaikkei sovelluksessasi viel‰ ole rooleja tai kirjautumista).

app.MapControllerRoute( //Eli kun k‰ytt‰j‰ menee selaimella osoitteeseen  https://localhost:xxxx,  sovellus ajaa: HomeController ? Index()
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages();



//k‰ynnist‰‰ sovelluksen ja alkaa kuunnella HTTP-pyyntˆj‰
app.Run();
