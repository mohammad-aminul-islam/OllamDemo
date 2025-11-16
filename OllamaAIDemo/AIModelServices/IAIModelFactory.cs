using OllamaAIDemo.AIModelServices;

namespace OllamaAIDemo;

public interface IAIModelFactory
{
    IAIModelService CreateAIModel(AIModelName aIModelName);
}
