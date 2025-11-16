namespace OllamaAIDemo.AIModelServices;

public class AIModelFactory : IAIModelFactory
{
    private readonly IServiceProvider _provider;

    public AIModelFactory(IServiceProvider provider)
    {
        _provider = provider;
    }
    public IAIModelService CreateAIModel(AIModelName aIModelName)
    {
        return _provider.GetRequiredKeyedService<IAIModelService>(aIModelName);
    }
}