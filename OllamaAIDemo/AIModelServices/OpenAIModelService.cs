using Microsoft.Extensions.Configuration;
using OpenAI;
using OpenAI.Chat;
using OllamaAIDemo.DTOs;
using OllamaAIDemo.MCPClientService;
using System.Runtime.CompilerServices;
using OllamaAIDemo.Configuration;
using Microsoft.Extensions.Options;

namespace OllamaAIDemo.AIModelServices;

public class OpenAIModelService : IAIModelService
{
    private readonly ChatClient _chatClient;
    private readonly IMcpClientService _mcpClientService;

    public AIModelName AIModelName { get; set; } = AIModelName.OpenAI;
    private readonly OpenAIConfiguration _openAIConfiguration;
    public OpenAIModelService(IOptions<OpenAIConfiguration> options, IMcpClientService mcpClientService)
    {
        _openAIConfiguration = options.Value;
        var openAIClient = new OpenAIClient(_openAIConfiguration.APIKey);
        _chatClient = openAIClient.GetChatClient(_openAIConfiguration.Model);
        
        _mcpClientService = mcpClientService;
    }

    public async IAsyncEnumerable<string> ChatAsync(
        ChatRequestDto request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var chatMessages = new List<ChatMessage>
        {
            new SystemChatMessage(SystemPrompt),
            new UserChatMessage(request.Prompt)
        };

        var options = new ChatCompletionOptions();

        // 1. Fetch Tools from MCP server
        var mcpClient = await _mcpClientService.GetClientAsync();
        var mcpToolsList = await mcpClient.ListToolsAsync(cancellationToken: cancellationToken);

        // 2. Map tools statically to the native OpenAI ChatTool
        foreach (var tool in mcpToolsList)
        {
            options.Tools.Add(ChatTool.CreateFunctionTool(
                functionName: tool.Name,
                functionDescription: tool.Description ?? "",
                functionParameters: BinaryData.FromObjectAsJson(tool.JsonSchema)
            ));
        }

        var responses = _chatClient.CompleteChatStreamingAsync(chatMessages, options, cancellationToken: cancellationToken);

        await foreach (var responseUpdate in responses)
        {
            foreach (var contentPart in responseUpdate.ContentUpdate)
            {
                if (!string.IsNullOrEmpty(contentPart.Text))
                {
                    Console.Write(contentPart.Text);
                    yield return contentPart.Text;
                }
            }
            
            // Note: In native OpenAI Client, handling ToolCalls is a manual process parsing responseUpdate.ToolCallUpdates.
        }
    }

    public async IAsyncEnumerable<string> AnalyzeAsync(
        ChatRequestDto request,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var chatMessages = new List<ChatMessage>
        {
            new SystemChatMessage("You are a data analysis assistant that extracts insights. Answer concisely."),
            new UserChatMessage(request.Prompt)
        };

        var responses = _chatClient.CompleteChatStreamingAsync(chatMessages, cancellationToken: cancellationToken);

        await foreach (var responseUpdate in responses)
        {
            foreach (var contentPart in responseUpdate.ContentUpdate)
            {
                if (!string.IsNullOrEmpty(contentPart.Text))
                {
                    Console.Write(contentPart.Text);
                    yield return contentPart.Text;
                }
            }
        }
    }

    private const string SystemPrompt = "You are an AI assistant. Analyze the inputs and provide a detailed response.";
}
