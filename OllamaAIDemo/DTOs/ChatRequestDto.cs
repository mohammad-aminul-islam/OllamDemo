using OllamaAIDemo.AIModelServices;
using System.Text.Json.Serialization;

namespace OllamaAIDemo.DTOs;

public class ChatRequestDto
{
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; }
    [JsonPropertyName("model")]
    public AIModelName Model { get; set; }
    [JsonPropertyName("attachment")]
    public IFormFile? Attachment { get; set; }
}
