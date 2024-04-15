﻿using Auth_API.Data;
using Auth_API.Models.Domain.Contact;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        public async Task<List<Contact>> GetAllAsync()
        {
            return await contactDbContext.Contacts.ToListAsync();
        }

        public async Task<Contact> GetByIdAsync(string id)
        {
            return await contactDbContext.Contacts.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Contact> CreateAsync(Contact contact)
        {
            await contactDbContext.Contacts.AddAsync(contact);
            await contactDbContext.SaveChangesAsync();

            return contact;
        }

        public async Task<Contact> DeleteAsync(string id)
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