namespace FiksComService.Models.Requests
{
    public class EditPasswordRequest
    {
        public required int UserId { get; set; }
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }

    }
}
