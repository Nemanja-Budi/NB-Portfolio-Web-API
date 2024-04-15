using Auth_API.Models.Domain.Contact;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auth_API.Repositories
{
    public interface IContactRepository
    {
        Task<List<Contact>> GetAllAsync(
            string? filterOn = null, 
            string? filterQuery = null, 
            string? sortBy = null, 
            bool isAscending = true, 
            int pageNumber = 1, 
            int pageSize = 1000
        );
        Task<Contact?> GetByIdAsync(string id);
        Task<Contact> CreateAsync(Contact contact);
        Task<Contact?> DeleteAsync(string id);

    }
}
