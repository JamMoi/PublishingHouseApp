using Microsoft.EntityFrameworkCore; //EF Core -kirjasto tietokantayhteyksi‰ varten
using Microsoft.Extensions.Options; //ei ehk‰ tarvita t‰ss‰, mutta usein hyˆdyllinen
using PublishingHouseApp.Data; //Jotta voidaan k‰yt‰‰ omaa tietokantakontekstia PublishingHouseContext
using PublishingHouseApp.Models; //Jotta voidaan k‰ytt‰‰ tietokantamalleja, kuten Book, Author jne.
using Microsoft.AspNetCore.Identity;


//yll‰ tuodaan tarvittavat kirjastot projektiin


var builder = WebApplication.CreateBuilder(args); //t‰ss‰ luodaan olio, johon lis‰t‰‰n palvelut (kuten tietokanta, MVC.jne) joka rakentaa web-sovelluksen


// lis‰t‰‰n palvelut sovellukseen
builder.Services.AddControllersWithViews(); //rekisterˆid‰‰n MVC-palvelut, jotta voidaan k‰ytt‰‰ ohjaimia ja n‰kymi‰
builder.Services.AddDbContext<PublishingHouseContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); //specifying the server to connect to the database

// Idnentity Frameworkin ja ASP.NET Identityn konfigurointi
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<PublishingHouseContext>()
.AddDefaultTokenProviders();

builder.Services.AddRazorPages();


var app = builder.Build(); //Rakennetaan varsinainen sovellus k‰ytt‰en edell‰ lis‰ttyj‰ palveluita.

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

    string[] roles = { "Admin", "User" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    var adminEmail = "admin@test.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser != null)
    {
        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
// Dummy-data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<PublishingHouseContext>();
    DbInitializer.Initialize(context);

}
    if (!app.Environment.IsDevelopment())
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
