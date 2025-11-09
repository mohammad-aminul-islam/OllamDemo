using Microsoft.Extensions.AI;
using OllamaAIDemo.DTOs;
using OllamaSharp;
using System.Runtime.CompilerServices;

namespace OllamaAIDemo.ChatClient;
public class ApplicationChatClient : IApplicationChatClient
{
    private readonly IChatClient _chatClient;

    public ApplicationChatClient(IChatClient chatClient)
    {
        this._chatClient = chatClient;
    }
    public async IAsyncEnumerable<string> ChatAsync(
        ChatRequestDto request,
        [EnumeratorCancellation]CancellationToken cancellationToken)
    {
        List<ChatMessage> chatHistory = new()
        {
            new ChatMessage(ChatRole.User, request.Prompt)
        };

        await foreach (var item in _chatClient.GetStreamingResponseAsync(
            chatHistory,
            cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(item.Text))
            {
                Console.WriteLine(item.Text);
                yield return item.Text;
            }
        }
    }

    public async IAsyncEnumerable<string> AnalyzeAsync(
        ChatRequestDto request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        List<ChatMessage> chatHistory = new()
        {
            new ChatMessage(ChatRole.User, request.Prompt),
            new ChatMessage(ChatRole.System, "You are a data analysis assistant that reads data and extracts insights. Answer exactly what is asked by user. Do not make mistake on calculation on salary data."),

        };

        await foreach (var item in _chatClient.GetStreamingResponseAsync(
            chatHistory,
            cancellationToken: cancellationToken))
        {
            if (!string.IsNullOrEmpty(item.Text))
            {
                Console.WriteLine(item.Text);
                yield return item.Text;
            }
        }
    }
}
