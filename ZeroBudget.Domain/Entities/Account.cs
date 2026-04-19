namespace ZeroBudget.Domain.Entities;

public class Account
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;

    private Account() { }

    public static Account Create(string name)
        => new()
        {
            Id = Guid.NewGuid(),
            Name = name.Trim()
        };

    public void Rename(string name) => Name = name.Trim();
}
