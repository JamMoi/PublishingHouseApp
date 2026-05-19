using System.Collections.Generic;
using PublishingHouseApp.Models;

namespace PublishingHouseApp.ViewModels
{
    public class BookEditViewModel //Tämä BookEditViewModel toimii kirjan muokkaussivun apumallina.
                                   //Se kokoaa samaan olioon kaiken tiedon, jota muokkauslomake tarvitsee.

//Siinä on:muokattavan kirjan tiedotkirjaan jo valittujen genrejen tunnisteet
//kaikki saatavilla olevat genret, joista käyttäjä voi valita
    {
       
        public Book Book { get; set; }         // Säilyttää sen kirjan tiedot, jota käyttäjä muokkaa lomakkeella.

        public List<int> SelectedGenreIds { get; set; } = new List<int>(); // Säilyttää niiden genrejen tunnisteet, jotka on valittu kyseiselle kirjalle.
        // Näiden avulla muokkauslomakkeessa voidaan näyttää oikeat genret valmiiksi valittuina.

        public List<Genre> AvailableGenres { get; set; } = new List<Genre>();         // Säilyttää kaikki genret, jotka näytetään käyttäjälle valittavina vaihtoehtoina muokkauslomakkeessa.



    }
}
  
