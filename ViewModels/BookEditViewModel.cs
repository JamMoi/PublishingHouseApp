using System.Collections.Generic;
using PublishingHouseApp.Models;

namespace PublishingHouseApp.ViewModels
{
    public class BookEditViewModel
    {
       
        public Book Book { get; set; }
        public List<int> SelectedGenreIds { get; set; } = new List<int>();
        public List<Genre> AvailableGenres { get; set; } = new List<Genre>();


} 
}
  
