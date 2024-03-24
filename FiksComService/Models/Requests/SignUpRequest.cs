namespace FiksComService.Models.Requests
{
    public class SignUpRequest
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
    }
}
