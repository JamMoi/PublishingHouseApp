using Microsoft.EntityFrameworkCore; //EF Core -kirjasto tietokantayhteyksiä varten
using Microsoft.Extensions.Options; //ei ehkä tarvita tässä, mutta usein hyödyllinen
using PublishingHouseApp.Data; //Jotta voidaan käytää omaa tietokantakontekstia PublishingHouseContext
using PublishingHouseApp.Models; //Jotta voidaan käyttää tietokantamalleja, kuten Book, Author jne.

//yllä tuodaan tarvittavat kirjastot projektiin


var builder = WebApplication.CreateBuilder(args); //tässä luodaan olio, johon lisätään palvelut (kuten tietokanta, MVC.jne) joka rakentaa web-sovelluksen

// lisätään palvelut sovellukseen
builder.Services.AddControllersWithViews(); //rekisteröidään MVC-palvelut, jotta voidaan käyttää ohjaimia ja näkymiä
builder.Services.AddDbContext<PublishingHouseContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); //specifying the server to connect to the database

var app = builder.Build(); //Rakennetaan varsinainen sovellus käyttäen edellä lisättyjä palveluita.

// HTTP pyyntöjen käsittelyn konfigurointi.
//Kaikki seuraavat määritykset vaikuttavat siihen, miten sovellus vastaa selaimen pyyntöihin.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    //Haetaan tietokantakonteksti

    var context = services.GetRequiredService<PublishingHouseContext>();

    // Kutsutaan DbInitializeria (lisää dummy-datan jos tietokanta tyhjä)
    DbInitializer.Initialize(context);
}
    if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // virheiden korjaus tuotannossa; ohjee virheet error sivulle
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts(); //käytetään HTTP Strict Transport Securityä, joka pakottaa selaimet käyttämään HTTPS:ää
}

app.UseHttpsRedirection();
app.UseStaticFiles(); //kartoitus staattisille resursseille kuten kuville, CSS:lle ja JavaScriptille
app.UseRouting();//Ottaa käyttöön reititysjärjestelmän (eli miten URLit ohjataan controllereille).

app.UseAuthorization(); // Aktivoi mahdollisen autorisoinnin (vaikkei sovelluksessasi vielä ole rooleja tai kirjautumista).

app.MapControllerRoute( //Eli kun käyttäjä menee selaimella osoitteeseen  https://localhost:xxxx,  sovellus ajaa: HomeController ? Index()
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

//käynnistää sovelluksen ja alkaa kuunnella HTTP-pyyntöjä
app.Run();
