using Auth_API.Models.Domain.Contact;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Auth_API.Repositories
{
    public interface IContactRepository
    {
        Task<List<Contact>> GetAllAsync();
        Task<Contact> GetByIdAsync(string id);
        Task<Contact> CreateAsync(Contact contact);
        Task<Contact> DeleteAsync(string id);

    }
}
