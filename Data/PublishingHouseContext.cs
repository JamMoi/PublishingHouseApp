using Microsoft.EntityFrameworkCore;
using PublishingHouseApp.Models;

namespace PublishingHouseApp.Data
{
    public class PublishingHouseContext : DbContext
    {
        public PublishingHouseContext(DbContextOptions<PublishingHouseContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
    }
}