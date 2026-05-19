using System.Collections.Generic;
using PublishingHouseApp.Models;

namespace PublishingHouseApp.ViewModels
{
    public class BookCreateViewModel // TÄmä viewModel toimii välimallina Create-sivun ja controllerin välillä
                                     //Sitä käytetään silloin kun uuden kirjan luontilomakkeessa tarvitaan muutakin kuin pelkkkä Book-olio.
                                     //Esimerkiksi, kun uuden kirjan luotisivulla tarvitaan adminin valitsemat genret, itse kirjan tiedot
    {
        // Säilyttää uuden kirjan tiedot, jotka käyttäjä syöttää lomakkeelle.
        // Uusi Book-olio luodaan valmiiksi, jotta sitä voidaan käyttää näkymässä turvallisesti.
        public Book Book { get; set; } = new Book();

        // Säilyttää käyttäjän lomakkeella valitsemien genrejen tunnisteet.
        // Tähän listaan tulee esimerkiksi valittujen genrejen GenreId-arvot.
        public List<int> SelectedGenreIds { get; set; } = new List<int>();

        // Säilyttää kaikki genret, jotka näytetään käyttäjälle valittavina vaihtoehtoina lomakkeessa.
        public List<Genre> AvailableGenres { get; set; } = new List<Genre>();
    }
}