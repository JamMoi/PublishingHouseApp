using Microsoft.EntityFrameworkCore;
using PublishingHouseApp.Models;

namespace PublishingHouseApp.Data
{
    public class PublishingHouseContext : DbContext
    {
        public PublishingHouseContext(DbContextOptions<PublishingHouseContext> options) //these are generic parameters that context needs
            : base(options) { }

       public  DbSet<Authors> Author {  get; set; }
        
        
    }
}
