namespace Auth_API.Models.Domain.Contact
{
    public class Contact
    {
        public string? Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public required string NameOfCompany { get; set; }
        public required string Email { get; set; }
        public required string Message { get; set; }
    }
}
