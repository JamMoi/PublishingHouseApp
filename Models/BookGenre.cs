namespace PublishingHouseApp.Models
{
    //liitostaulu joka yhdistää Book- ja Genre-luokat monen-moneen-suhteessa. Jokaisella BookGenre-oliolla on viite yhteen kirjaan ja yhteen genreen. Tämä mahdollistaa sen, että yksi kirja voi kuulua useaan genreeseen ja yksi genre voi sisältää useita kirjoja.
    public class BookGenre // Tämä luokka edustaa monen-moneen-suhdetta kirjojen ja genrejen välillä. Jokaisella BookGenre-oliolla on viite yhteen kirjaan ja yhteen genreen.
    {
        public int BookId { get; set; } // Viite kirjaan
        public Book Book { get; set; } = null!; // Navigointiominaisuus kirjaan
        public int GenreId { get; set; } // Viite genreen
        public Genre Genre { get; set; } = null!; // Navigointiominaisuus genreen
    }
}
