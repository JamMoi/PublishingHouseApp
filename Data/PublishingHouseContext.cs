using Microsoft.AspNetCore.Identity.EntityFrameworkCore;// Tuo käyttöön ASP.NET Core Identity -tietokantakontextin.IdentityDbContext-luokka tarjoaa valmiit taulut ja toiminnallisuudet käyttäjien, roolien ja muiden identiteettiin liittyvien tietojen hallintaan. Perimällä tästä luokasta, PublishingHouseContext saa automaattisesti kaikki nämä ominaisuudet.
using Microsoft.EntityFrameworkCore; //tarvitaan Entity Framework Core -toiminnallisuuksiin, kuten DbContext, DbSet, ja LINQ-kyselyihin. Tämä mahdollistaa tietokantayhteyden ja tietokantataulujen määrittelyn C#-koodissa
using PublishingHouseApp.Models;// Tuo käyttöön projektin omat mallit, kuten Book, Author, Genre, BookGenre. Näitä malleja käytetään tietokantataulujen määrittelyyn ja relaation luomiseen niiden välillä

namespace PublishingHouseApp.Data
{
    // ApplicationDbContext on sovelluksen tietokantakonteksti
    // IdentityDbContext tarkoittaa, että mukana ovat myös käyttäjät ja roolit.
    public class PublishingHouseContext : IdentityDbContext<ApplicationUser>
    {
        
        // Konstruktorissa vastaanotetaan tietokannan asetukset
        // ja ne välitetään eteenpäin IdentityDbContextille.
        public PublishingHouseContext(DbContextOptions<PublishingHouseContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }        // Books-taulu tietokannassa

        public DbSet<Author> Authors { get; set; }

        public DbSet<Genre> Genres { get; set; }        // Genres-taulu tietokannassa

        public DbSet <BookGenre> BookGenres { get; set; }        // BookGenres-liitostaulu, joka yhdistää kirjat ja genret

        protected override void OnModelCreating(ModelBuilder builder)//        // Täällä määritellään tarkemmin, miten taulut liittyvät toisiinsa

        {
            base.OnModelCreating(builder);            // Identity tarvitsee tämän rivin, muuten käyttäjien kirjautuminen voi hajota
                                                      // Näin samaa genreä ei voi lisätä samalle kirjalle monta kertaa.

            builder.Entity<BookGenre>() // Määritellään BookGenre-taulu
                .HasKey(bg => new { bg.BookId, bg.GenreId }); // Määritellään yhdistelmäavain (BookId + GenreId)
        
            builder.Entity<BookGenre>() // Määritellään suhde BookGenre-taulun ja Book-taulun välillä
                .HasOne(bg => bg.Book) // Jokaisella BookGenre-oliolla on yksi Book
                .WithMany(b => b.BookGenres) // Jokaisella Book-oliolla voi olla monta BookGenre-oliota
                .HasForeignKey(bg => bg.BookId); // Foreign key on BookId

            builder.Entity<BookGenre>() // Määritellään suhde BookGenre-taulun ja Genre-taulun välillä
                .HasOne(bg => bg.Genre) // Jokaisella BookGenre-oliolla on yksi Genre
                .WithMany(g => g.BookGenres) // Jokaisella Genre-oliolla voi olla monta BookGenre-oliota
                .HasForeignKey(bg => bg.GenreId); // Foreign key on GenreId

            builder.Entity<Genre>().HasData(
    new Genre { Id = 1, Name = "Romaani" },
    new Genre { Id = 2, Name = "Dekkari" },
    new Genre { Id = 3, Name = "Trilleri" },
    new Genre { Id = 4, Name = "Fantasia" },
    new Genre { Id = 5, Name = "Tieteiskirjallisuus" },
    new Genre { Id = 6, Name = "Kauhu" },
    new Genre { Id = 7, Name = "Romantiikka" },
    new Genre { Id = 8, Name = "Historiallinen fiktio" },
    new Genre { Id = 9, Name = "Nuortenkirja" },
    new Genre { Id = 10, Name = "Lastenkirja" },
    new Genre { Id = 11, Name = "Elämäkerta" },
    new Genre { Id = 12, Name = "Historia" },
    new Genre { Id = 13, Name = "Tiede" },
    new Genre { Id = 14, Name = "Itseapu" },
    new Genre { Id = 15, Name = "Ruoanlaitto" },
    new Genre { Id = 16, Name = "Matkailu" },
    new Genre { Id = 17, Name = "Sarjakuva" },
    new Genre { Id = 18, Name = "Manga" },
    new Genre { Id = 19, Name = "Sci-Fi" }
);
        }

    }

}