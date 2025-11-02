namespace OllamaAIDemo.ChatClient;

public interface IApplicationChatClient
{
    Task<string> ChatAsync(string prompt, CancellationToken cancellationToken);
}
