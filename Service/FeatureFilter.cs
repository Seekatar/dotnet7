using Microsoft.FeatureManagement;

namespace dotnet7.Service;

public class MyFeatureContext
{
    
}

[FilterAlias("FilterMe")]
public class FeatureFilter : IContextualFeatureFilter<MyFeatureContext>
{
    public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context, MyFeatureContext mycontext)
    {
        if (context.FeatureName == "TEST.KEYQ") throw new Exception("Ow!");
        return Task.FromResult(true);
    }
}