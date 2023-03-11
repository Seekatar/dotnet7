namespace dotnet7.FeatureFlags;

interface IToggledFeature
{
    string GetResult();
}

public class ToggledFeatureA : IToggledFeature
{
    public string GetResult()
    {
        return $"This is from {GetType().Name}";
    }
}

public class ToggledFeatureB : ToggledFeatureA
{
}

