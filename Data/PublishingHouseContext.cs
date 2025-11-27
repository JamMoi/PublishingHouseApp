using Microsoft.EntityFrameworkCore; //  → antaa käyttöön Entity Framework Coren (EF Core), jota käytetään tietokantayhteyksiin.
using PublishingHouseApp.Models; //→ jotta voidaan käyttää Model-luokkia (Authors, Books, Employees)
using PublishingHouseApp.Data;


namespace PublishingHouseApp.Data
{

    //tämä konteksti Luokka on yhteys tietokantaan ja määrittelee DbSetit
    //mahdollistaa tietojen kyselyn ja tallennuksen tietokantaan
    public class PublishingHouseContext : DbContext //tässä luodaan tietokantakonteksti, joka perii EF coren DBContext-luokan
    {
        public PublishingHouseContext(DbContextOptions<PublishingHouseContext> options) //these are generic parameters that context needs
            : base(options) { }
              
        public DbSet<Employees> Employees { get; set; }

        public DbSet<PublishingHouseApp.Models.Author> Authors { get; set; } = default!; //➡ Luo tietokantaan taulun nimeltä Author, joka sisältää Authors-mallin kentät (Id, Firstname, Surname, Email jne.).Tämän avulla voit esim. hakea kaikki kirjoittajat: var authors = context.Author.ToList();

        public DbSet<PublishingHouseApp.Models.Book> Books { get; set; } = default!;//vastaa tietokanan Book-taulua


    }
}
