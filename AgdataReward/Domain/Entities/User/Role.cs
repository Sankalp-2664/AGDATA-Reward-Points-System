namespace Domain.Entities.User;

public class Role
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    protected Role() { }

    public Role(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Role name cannot be empty");

        Id = Guid.NewGuid();
        Name = name.Trim();
    }
}
