using Auth_API.Data;
using Auth_API.Models.Domain.Contact;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth_API.Repositories
{
    public class SQLContactRepository : IContactRepository
    {
        private readonly ContactDbContext contactDbContext;

        public SQLContactRepository(ContactDbContext contactDbContext)
        {
            this.contactDbContext = contactDbContext;
        }

        public async Task<List<Contact>> GetAllAsync(string? filterOn = null, string? filterQuery = null, string? sortBy = null, bool isAscending = true)
        {
            IQueryable<Contact> contacts = contactDbContext.Contacts;

            if (!string.IsNullOrWhiteSpace(filterOn) && !string.IsNullOrWhiteSpace(filterQuery))
            {
                if (filterOn.Equals("NameOfCompany", StringComparison.OrdinalIgnoreCase))
                {
                    contacts = contacts.Where(x => x.NameOfCompany.Contains(filterQuery));
                }
            }

            if(string.IsNullOrWhiteSpace(sortBy) == false)
            {
                if(sortBy.Equals("NameOfCompany", StringComparison.OrdinalIgnoreCase))
                {
                    contacts = isAscending ? contacts.OrderBy(x => x.NameOfCompany) : contacts.OrderByDescending(x => x.NameOfCompany);
                }               
            }

            return await contacts.ToListAsync();
        }


        public async Task<Contact?> GetByIdAsync(string id)
        {
            return await contactDbContext.Contacts.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Contact> CreateAsync(Contact contact)
        {
            await contactDbContext.Contacts.AddAsync(contact);
            await contactDbContext.SaveChangesAsync();

            return contact;
        }

        public async Task<Contact?> DeleteAsync(string id)
        {
            var existingContact = await contactDbContext.Contacts.FirstOrDefaultAsync(x => x.Id == id);
            if (existingContact == null) 
            {
                return null;
            }

            contactDbContext.Contacts.Remove(existingContact);
            await contactDbContext.SaveChangesAsync();

            return existingContact;
        }
    }
}
