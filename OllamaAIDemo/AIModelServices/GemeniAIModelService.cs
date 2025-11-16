using Google.GenAI;
using OllamaAIDemo.DTOs;

namespace OllamaAIDemo.AIModelServices;

public class GemeniAIModelService : IAIModelService
{
    public AIModelName AIModelName { get; set; } = AIModelName.Gemini;
    private const string APIKey = "";
    public IAsyncEnumerable<string> AnalyzeAsync(
        ChatRequestDto request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async IAsyncEnumerable<string> ChatAsync(
        ChatRequestDto request,
        CancellationToken cancellationToken)
    {
        var client = new Client(apiKey: APIKey,project: "gen-lang-client-0536838725");
        await foreach (var response in client.Models.GenerateContentStreamAsync(
           model: "gemini-2.0-flash", contents: request.Prompt
         ))
        {
            yield return response?.Candidates[0]?.Content?.Parts[0]?.Text ?? "";
        }
    }
}
