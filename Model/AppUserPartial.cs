namespace Model;

#if !Packaging

interface IAuditable
{
    string AuditName { get; set; }
}

public partial class AppUser : IAuditable
{
    public string AuditName { get; set; } = "Hi, I'm a partial class!2";
}

#endif