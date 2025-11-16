using OllamaAIDemo.DTOs;
using System.Runtime.CompilerServices;

namespace OllamaAIDemo.AIModelServices;

public interface IAIModelService
{
    public AIModelName AIModelName { get; set; }
    IAsyncEnumerable<string> ChatAsync(ChatRequestDto request, CancellationToken cancellationToken);
    IAsyncEnumerable<string> AnalyzeAsync(ChatRequestDto request, CancellationToken cancellationToken);
}
