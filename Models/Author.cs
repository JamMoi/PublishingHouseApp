using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PublishingHouseApp.Models;

// Tämä luokka vastaa yhtä kirjailija -riviä tietokannassa.
// EF Core käyttää tätä mallia Author-taulun luomiseen ja CRUD-OPERAATIOIHIN (Create, Read, Update, Delete).
public partial class Author //partial tarkoittaa, että samaa luokkaa voidaan määritellä useassa tiedostossa (esim. automaattisesti generoitu EF Core -koodi ja oma koodi).
{
    //Pääavain, joka tunnistaa yksilöllisesti jokaisen kirjailijan tietokannassa.
    //Tämä arvo generoidaan automaattisesti, kun uusi kirjailija lisätään.
    [Key] // kertoo EF-.lle että tämä on pääavain
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // automaattisesti generoitu ID
    public int AuthorId { get; set; }//yksilöllinen tunniste jokaiselle kirjailijalle.
    [Required] 

    public string Firstname { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Bio { get; set; } = null!;

    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.Today;

    // Navigointiominaisuus
    // Tämä sisältää kaikki kirjailijan kirjoittamat kirjat.
    // Ef Core luo 1-to-many -suhteen: Author -> Books. käyttää tätä Delete-logiikassa
    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
