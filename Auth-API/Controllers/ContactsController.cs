using Auth_API.Data;
using Auth_API.Models.Domain.Contact;
using Auth_API.Models.DTOs.Contact;
using Auth_API.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Auth_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactRepository contactRepository;

        public ContactsController(IContactRepository contactRepository)
        {
            this.contactRepository = contactRepository;
        }

        [HttpPost("add-new-contact")]
        public async Task<IActionResult> CreateContact([FromBody] ContactDto contactDTO)
        {
            var contactDomainModel = new Contact
            {
                Id = Guid.NewGuid().ToString(),
                NameOfCompany = contactDTO.NameOfCompany,
                Email = contactDTO.Email,
                Message = contactDTO.Message
            };

            contactDomainModel = await contactRepository.CreateAsync(contactDomainModel);

            var contactDto = new ContactDto
            {
                Id = contactDomainModel.Id,
                NameOfCompany = contactDomainModel.NameOfCompany,
                Email = contactDomainModel.Email,
                Message = contactDomainModel.Message
            };

            return CreatedAtAction(nameof(GetContactById), new { id = contactDto.Id}, contactDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-contact/{id}")]
        public async Task<IActionResult> GetContactById(string id)
        {
            var contactDomain = await contactRepository.GetByIdAsync(id);

            if (contactDomain == null)
            {
                return NotFound();
            }

            var contactDto = new ContactDto
            {
                Id = contactDomain.Id,
                NameOfCompany = contactDomain.NameOfCompany,
                Email = contactDomain.Email,
                Message = contactDomain.Message
            };

            return Ok(contactDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-contacts")]
        public async Task<IActionResult> GetAllContacts()
        {
            var contactsDomain = await contactRepository.GetAllAsync();
            var contactDto = new List<ContactDto>();

            foreach (var contactDomain in contactsDomain)
            {
                contactDto.Add(new ContactDto()
                {
                    Id = contactDomain.Id,
                    NameOfCompany = contactDomain.NameOfCompany,
                    Email = contactDomain.Email,
                    Message = contactDomain.Message,
                });
            }

            return Ok(contactDto);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete-contact/{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var existingContact = await contactRepository.DeleteAsync(id);

            if (existingContact == null)
            {
                return NotFound();
            }

            var contactDto = new ContactDto
            {
                Id = existingContact.Id,
                NameOfCompany = existingContact.NameOfCompany,
                Email = existingContact.Email,
                Message = existingContact.Message,
            };

            return Ok(contactDto);
        }
    }
}
