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

    public int AuthorId { get; set; }

    public string Isbn { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Editor { get; set; } = null!;

    public DateTime PublishedDate { get; set; }

    public string Edition { get; set; } = null!;

    public string Genre { get; set; } = null!;

    public string Language { get; set; } = null!;

    public string Keywords { get; set; } = null!;

    public int? PageCount { get; set; }

    public string Format { get; set; } = null!;

    public string FilePath { get; set; } = null!;

    public string CoverImageUrl { get; set; } = null!;

    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    public bool IsAvailable { get; set; }

   public virtual Author Author { get; set; } = null!;
}
