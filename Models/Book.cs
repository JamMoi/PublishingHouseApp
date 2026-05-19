using Humanizer;//tarjoaa metodeja merkkijonojen käsittelyyn, kuten muuntamaan tekstiä eri muotoihin (esim.päivämäärät monikko, isomuoto jne.).
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;//mahdollistaa attribuutit, kuten [Key], [Required].
using System.ComponentModel.DataAnnotations.Schema;//attribuutit, jotka vaikuttavat tietokantaan, esim. [DatabaseGenerated].

namespace PublishingHouseApp.Models;

public partial class Book //Book-luokka edustaa tietokannan Book-taulua.
{
   [Key]
   [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // automaattisesti generoitu BookId
    public int BookId { get; set; } //primary key
    [Required]
    public int AuthorId { get; set; }// Viittaa Author-taulukkoon

    public virtual Author? Author { get; set; }//Navigointiominaisuus Author-malliin (many-to-one-suhde).Author-ominaisuus mahdollistaa, että controller ja view voivat näyttää kirjailijan nimen, esim. item.Author.Firstname.Tämä luo relaation Book → Author(many-to-one).???

    public string Isbn { get; set; } = null!; //alla Kaikki kirjan ominaisuudet.

    [Required] 
    public string Title { get; set; } = null!;

    [DataType(DataType.Date)]
    public DateTime? PublishedDate { get; set; }
    public decimal Price { get; set; }
    [Required] 
    public bool IsAvailable { get; set; }


    public string? Editor { get; set; }


    public string? Edition { get; set; }

    //public string? Genre { get; set; }
    public ICollection<BookGenre> BookGenres { get; set; } = new List<BookGenre>();

    public string? Language { get; set; }

    public string? Keywords { get; set; }

    public int? PageCount { get; set; }
        
    public string? Format { get; set; }

    public string? FilePath { get; set; }

    public string? CoverImageUrl { get; set; }




}
