namespace dotnet7.FeatureFlagDemo;

public record FeatureContext
{
    public string? ClientId { get; set; }
    public int? MarketId { get; set; }
}
