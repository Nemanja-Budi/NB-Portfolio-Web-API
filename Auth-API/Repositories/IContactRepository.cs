using Auth_API.Models.Domain.Contact;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auth_API.Repositories
{
    public interface IContactRepository
    {
        Task<List<Contact>> GetAllAsync(string? filterOn = null, [FromQuery] string? filterQuery = null);
        Task<Contact?> GetByIdAsync(string id);
        Task<Contact> CreateAsync(Contact contact);
        Task<Contact?> DeleteAsync(string id);

    }
}
