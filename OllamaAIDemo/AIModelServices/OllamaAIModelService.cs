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
        if (request.Attachment != null)
        {
            using var ms = new MemoryStream();
            await request.Attachment.CopyToAsync(ms);
            var fileBytes = ms.ToArray();
            var readonlyms = new ReadOnlyMemory<byte>(fileBytes);
            userMessage.Contents.Add(new DataContent(readonlyms,
                                                 request.Attachment.ContentType ?? "application/octet-stream"));
        }
        //userMessage.Contents.Add(new DataContent(File.ReadAllBytes("Images/traffic02.png"), "image/png"));
        List<ChatMessage> chatHistory = new()
        {
            userMessage,
            new ChatMessage(ChatRole.System,SystemPrompt)
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
                                       // Danger First: If there is any danger, hazard, or risky situation in the image, start your response with ""ALERT:"" followed by a short warning.
    private const string SystemPrompt = @"
                                        You are an AI image analyst. Analyze the image and provide a detailed response. Follow these instructions:

                                        1. Identify the main objects or subjects in the image.
                                        2. Describe their attributes (color, size, shape, position, orientation).
                                        3. Identify any actions, interactions, or activities happening.
                                        4. Recognize text, symbols, or logos if present.
                                        5. Summarize the overall scene and context.
                                        6. Provide insights, potential use cases, or anomalies if any.

                                        Image metadata or description: 
                                        The image can be of any type, including but not limited to landscapes, urban scenes, people, animals, objects, or abstract art.
                                        Answer in a structured format:
                                        - Objects:
                                        - Attributes:
                                        - Actions:
                                        - Text/Logos:
                                        - Scene Summary:
                                        - Insights:
                                            
                                        ";
}
