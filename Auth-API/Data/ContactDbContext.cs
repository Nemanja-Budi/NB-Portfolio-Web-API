using Auth_API.Models.Domain.Contact;
using Microsoft.EntityFrameworkCore;

namespace Auth_API.Data
{
    public class ContactDbContext : DbContext
    {
        public ContactDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {
            
        }

        public DbSet<Contact> Contacts { get; set; }
    }
}
