using System;
using System.Linq;
using PublishingHouseApp.Models;

namespace PublishingHouseApp.Data
{
    // Tämä luokka lisää tietokantaan esimerkkidataa,
    // jos tietokanta on tyhjä.
    public static class DbInitializer
    {
        public static void Initialize(PublishingHouseContext context)
        {
            // Varmistaa, että tietokanta on luotu
            // (jos ei ole, se luodaan automaattisesti)
            context.Database.EnsureCreated();

            // Jos Books-taulussa on jo dataa,
            // oletetaan että tietokanta on alustettu
            if (context.Books.Any())
            {
                return; // Ei tehdä mitään
            }

            // ------------------ Lisätään kirjailijat ------------------

            var authors = new Author[]
            {
                new Author
                {
                    Firstname = "Minna",
                    Surname = "Salakari",
                    Email = "minna@publishing.com",
                    Date = DateTime.Today
                },
                new Author
                {
                    Firstname = "Suvi",
                    Surname = "Siikalahti",
                    Email = "suvi@publishing.com",
                    Date = DateTime.Today
                }
            };

            // Tallennetaan authorit tietokantaan
            context.Authors.AddRange(authors);
            context.SaveChanges();

            // Haetaan tallennetut authorit,
            // jotta saadaan niiden AuthorId-arvot
            var author1 = context.Authors.First();
            var author2 = context.Authors.Skip(1).First();

            // ------------------ Lisätään dummy-kirjat ------------------

            var books = new Book[]
            {
                new Book
                {
                    Title = "Lasisilmä",
                    Price = 20m,
                    IsAvailable = true,
                    PublishedDate = DateTime.Today,
                    AuthorId = author1.AuthorId,

                    // Suora kuvalinkki kansikuvaan
                    CoverImageUrl = "https://fastly.picsum.photos/id/970/200/300.jpg?hmac=8mPwdPFtAKcn0NQrEIClW3IlOWsKgskAikm_8YQj-qM"
                },

                new Book
                {
                    Title = "Meidän maailma",
                    Price = 46m,
                    IsAvailable = true,
                    PublishedDate = DateTime.Today.AddDays(-1),
                    AuthorId = author1.AuthorId,

                    // Placeholder-kuva
                    CoverImageUrl = "https://via.placeholder.com/400x600?text=Book+2"
                },

                new Book
                {
                    Title = "Kuun kissa",
                    Price = 15m,
                    IsAvailable = true,
                    PublishedDate = DateTime.Today.AddDays(-2),
                    AuthorId = author2.AuthorId,

                    CoverImageUrl = "https://via.placeholder.com/400x600?text=Book+3"
                },

                new Book
                {
                    Title = "Heinikko",
                    Price = 12.50m,
                    IsAvailable = true,
                    PublishedDate = DateTime.Today.AddDays(-3),
                    AuthorId = author2.AuthorId,

                    CoverImageUrl = "https://via.placeholder.com/400x600?text=Book+4"
                }
            };

            // Tallennetaan kirjat tietokantaan
            context.Books.AddRange(books);
            context.SaveChanges();
        }
    }
}