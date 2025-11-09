using System.Text.Json.Serialization;

namespace OllamaAIDemo.DTOs;

public class ChatRequestDto
{
    [JsonPropertyName("prompt")]
    public string Prompt { get; set; }
    [JsonPropertyName("model")]
    public string Model { get; set; }
}
