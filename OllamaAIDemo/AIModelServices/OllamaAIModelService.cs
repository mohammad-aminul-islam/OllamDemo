using Microsoft.Extensions.AI;
using OllamaAIDemo.DTOs;
using OllamaSharp;
using System.Runtime.CompilerServices;

namespace OllamaAIDemo.AIModelServices;
public class OllamaAIModelService : IAIModelService
{
    private readonly IChatClient _chatClient;

    public AIModelName AIModelName { get; set; } = AIModelName.Ollama;

    public OllamaAIModelService(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }
    public async IAsyncEnumerable<string> ChatAsync(
        ChatRequestDto request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var userMessage = new ChatMessage(ChatRole.User, request.Prompt);
        userMessage.Contents.Add(new DataContent(File.ReadAllBytes("Images/traffic02.png"), "image/png"));
        List<ChatMessage> chatHistory = new()
        {
            userMessage,
            new ChatMessage(ChatRole.System,"You are expert as an information provider. Always format the data properly, if needed show data as order list.")
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
