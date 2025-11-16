using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace OllamaAIDemo.Services;

public class OpenAIEmbeddingService
{
    private readonly string OpenAIAPIKey = "";
    private readonly IHttpClientFactory _httpClientFactory;
    private const string BaseUrl = "https://api.openai.com/v1/embeddings";
    public OpenAIEmbeddingService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<(string errorMessage, EmbeddingResponse? data)> GenerateVectorEmbedding(string text)
    {
        try
        {
            var _httpClient = _httpClientFactory.CreateClient();
            var payload = new
            {
                input = text,
                model = "text-embedding-ada-002",
                encoding_format = "float"
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            // Add Authorization header
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", OpenAIAPIKey);
            var response = await _httpClient.PostAsync(BaseUrl, content);
            if (response == null)
                return new("No response found", null);

            if (response.IsSuccessStatusCode)
            {
                var sresult = await response.Content.ReadAsStringAsync();
                var sdata = JsonSerializer.Deserialize<EmbeddingResponse>(sresult);
                return (string.Empty, sdata);
            }
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine(result);
            return new(result, null);
        }
        catch (Exception ex)
        {
            return new($"{ex.Message}, {ex.StackTrace}", null);
            throw;
        }
    }
}

public class VectorEmbeddingCreateDto
{
    public string input { get; set; }
    public string model { get; set; }
    public string encoding_format { get; set; }
}

public class EmbeddingResponse
{
    public string Object { get; set; }
    public List<EmbeddingData> Data { get; set; }
    public string Model { get; set; }
    public Usage Usage { get; set; }
}

public class EmbeddingData
{
    public string Object { get; set; }
    public List<float> Embedding { get; set; }
    public int Index { get; set; }
}

public class Usage
{
    public int Prompt_Tokens { get; set; }
    public int Total_Tokens { get; set; }
}

public class EmbeddingFailedResponse
{
    public Error error { get; set; }
}

public class Error
{
    public string message { get; set; }
    public string type { get; set; }
    public object param { get; set; }
    public string code { get; set; }
}
