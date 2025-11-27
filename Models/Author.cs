using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace PublishingHouseApp.Models;

public partial class Author
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // automaattisesti generoitu numero
    public int AuthorId { get; set; }
    [Required] 

    public string Firstname { get; set; } = null!;

    public string Surname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PhoneNumber { get; set; } = null!;

    public string Bio { get; set; } = null!;

    [DataType(DataType.Date)]
    public DateTime? Date { get; set; } = DateTime.Today;
  

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
