using Microsoft.EntityFrameworkCore; //  → antaa käyttöön Entity Framework Coren (EF Core), jota käytetään tietokantayhteyksiin.
using PublishingHouseApp.Models; //→ jotta voidaan käyttää Model-luokkia (Authors, Books, Employees)
using PublishingHouseApp.Data; // → pääsy projektin Data-kansioon ja sen sisältämiin luokkiin (esim. tämä konteksti itse).

namespace PublishingHouseApp.Data
{

    //tämä konteksti Luokka on yhteys tietokantaan ja määrittelee DbSetit
    //mahdollistaa tietojen kyselyn ja tallennuksen tietokantaan
    public class PublishingHouseContext : DbContext //tässä luodaan tietokantakonteksti, joka perii EF coren DBContext-luokan.
                                                    
    {
        public PublishingHouseContext(DbContextOptions<PublishingHouseContext> options) //these are generic parameters that context needs
            : base(options) { }

        // ------------------ DBSetit = Tietokantataulut ------------------

        public DbSet<Employees> Employees { get; set; }

        public DbSet<PublishingHouseApp.Models.Author> Authors { get; set; } = default!; //➡ Luo tietokantaan taulun nimeltä Author, joka sisältää Authors-mallin kentät (Id, Firstname, Surname, Email jne.).Tämän avulla voit esim. hakea kaikki kirjoittajat: var authors = context.Author.ToList();

        public DbSet<PublishingHouseApp.Models.Book> Books { get; set; } = default!;//vastaa tietokanan Book-taulua

        // ------------------ Mallien lisämäärittelyt ------------------

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Määritellään yksi-moneen -suhde:
            // Yhdellä kirjailijalla voi olla monta kirjaa (Books).
            modelBuilder.Entity<Author>()
                .HasMany(a => a.Books) // Authorilla on monta kirjaa
                .WithOne(b => b.Author) //Jokaisella kirjalla on yksi kirjailija
                .HasForeignKey(b => b.AuthorId) //Ulkoinen avain Book-taulussa
                .OnDelete(DeleteBehavior.Restrict); // Estetään Authorin poistaminen, jos kirjoja on

            //BookId luodaan automaattisesti tietokannassa
            modelBuilder.Entity<Book>()
                .Property(b => b.BookId)
                .ValueGeneratedOnAdd(); // EF tietää että BookId tulee tietokannasta automaattisesti (IDENTITY)
                                        //lomakkeisiin ei tarvitse lisästä BookID kenttää

            // PublishedDate tallennetaan SQL Serverissä datetime2-tyyppinä

            modelBuilder.Entity<Book>()
                .Property(b => b.PublishedDate)
                .HasColumnType("datetime2");
        }
    }
}
    


