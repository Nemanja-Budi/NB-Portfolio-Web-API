using Auth_API.Models.Domain.Contact;
using Auth_API.Models.DTOs.Contact;
using AutoMapper;

namespace Auth_API.Mappings
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Contact, ContactDto>().ReverseMap();
        }
    }
}
