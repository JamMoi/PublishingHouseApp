using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PublishingHouseApp.Models;

public partial class Book
{
   [Key]
   [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // automaattisesti generoitu BookId
    public int BookId { get; set; } //primary key
    [Required]
    public int AuthorId { get; set; }

    public virtual Author? Author { get; set; }

    public string Isbn { get; set; } = null!;

    [Required] 
    public string Title { get; set; } = null!;

    public DateTime PublishedDate { get; set; }
    public decimal Price { get; set; }
    [Required] 
    public bool IsAvailable { get; set; }


    public string? Editor { get; set; }


    public string? Edition { get; set; }

    public string? Genre { get; set; }

    public string? Language { get; set; }

    public string? Keywords { get; set; }

    public int? PageCount { get; set; }
        
    public string? Format { get; set; }

    public string? FilePath { get; set; }

    public string? CoverImageUrl { get; set; }




}
