using System.Collections.Generic;
using PublishingHouseApp.Models;

namespace PublishingHouseApp.ViewModels
{
    public class BookCreateViewModel
    {
        public Book Book { get; set; } = new Book();

        public List<int> SelectedGenreIds { get; set; } = new List<int>();

        public List<Genre> AvailableGenres { get; set; } = new List<Genre>();
    }
}