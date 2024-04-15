using Auth_API.Data;
using Auth_API.Models.Domain.Contact;
using Auth_API.Models.DTOs.Contact;
using Auth_API.Repositories;
using AutoMapper;
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
        private readonly IMapper mapper;

        public ContactsController(IContactRepository contactRepository,IMapper mapper)
        {
            this.contactRepository = contactRepository;
            this.mapper = mapper;
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

            var contactDto = mapper.Map<ContactDto>(contactDomainModel);

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
                
            return Ok(mapper.Map<ContactDto>(contactDomain));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("get-contacts")]
        public async Task<IActionResult> GetAllContacts([FromQuery] string? filterOn, [FromQuery] string? filterQuery, [FromQuery] string? sortBy, [FromQuery] bool? isAscending)
        {
            var contactsDomain = await contactRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true);

            return Ok(mapper.Map<List<ContactDto>>(contactsDomain));
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

            return Ok(mapper.Map<ContactDto>(existingContact));
        }
    }
}
