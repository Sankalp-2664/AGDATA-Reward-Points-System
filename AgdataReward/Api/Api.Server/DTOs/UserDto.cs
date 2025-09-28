namespace Api.Server.DTOs
{
    public class UserDto
    {
        public Guid UserId { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int RewardBalance { get; set; }
    }

}
