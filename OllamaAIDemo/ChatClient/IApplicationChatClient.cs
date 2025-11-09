using OllamaAIDemo.DTOs;
using System.Runtime.CompilerServices;

namespace OllamaAIDemo.ChatClient;

public interface IApplicationChatClient
{
    IAsyncEnumerable<string> ChatAsync(ChatRequestDto request, CancellationToken cancellationToken);
}
