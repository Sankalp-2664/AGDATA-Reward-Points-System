namespace Domain.Entities.User;

public class UserRole
{
    public Guid UserId { get; private set; }
    public UserProfile User { get; private set; } = null!;

    public Guid RoleId { get; private set; }
    public Role Role { get; private set; } = null!;

    protected UserRole() { }

    public UserRole(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}
