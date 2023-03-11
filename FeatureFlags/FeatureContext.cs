namespace dotnet7.FeatureFlags;

public record FeatureContext
{
    public bool EnableMe { get; set; }
    public string? ClientId { get; set; }
    public int? MarketId { get; set; }
}
