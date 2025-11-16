using Microsoft.AspNetCore.Mvc;
using OllamaAIDemo.DTOs;
using OllamaAIDemo.Services;
using System.Text.Json;

namespace OllamaAIDemo.Controllers;

[Route("api/chats")]
[ApiController]
public class ChatApiController : ControllerBase
{
    private readonly IAIModelFactory _factory;
    public ChatApiController(IAIModelFactory applicationChatClient)
    {
        this._factory = applicationChatClient;
    }

    [HttpPost]
    public async Task ChatAsync(
        [FromBody] ChatRequestDto request,
        CancellationToken cancellationToken)
    {
        Response.Headers.TryAdd("Content-Type", "text/event-stream");
        Response.Headers.TryAdd("Cache-Control", "no-cache");
        Response.Headers.TryAdd("Connection", "keep-alive");

        try
        {
            var selectedModel = _factory.CreateAIModel(request.Model);
            await foreach (var chunk in selectedModel.ChatAsync(request, cancellationToken))
            {
                var json = JsonSerializer.Serialize(new { content = chunk, done = false });
                await Response.WriteAsync($"data: {json}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                var doneJson = JsonSerializer.Serialize(new { content = "", done = true });
                await Response.WriteAsync($"data: {doneJson}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Client disconnected - this is normal, just log it
            Console.WriteLine("Client cancelled the request");
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Console.WriteLine($"Error during streaming: {ex.Message}");
            throw;
        }
    }


    [HttpPost("analyze")]
    public async Task AnalyzeAsync(
        [FromBody] ChatRequestDto request,
        [FromServices] EmployeeDataProvider employeeDataProvider,
        CancellationToken cancellationToken)
    {
        Response.Headers.TryAdd("Content-Type", "text/event-stream");
        Response.Headers.TryAdd("Cache-Control", "no-cache");
        Response.Headers.TryAdd("Connection", "keep-alive");

        try
        {
            var selectedModel = _factory.CreateAIModel(request.Model);

            var employees = employeeDataProvider.GetEmployees();
            var context = employeeDataProvider.BuildContextPrompt(employees, request.Prompt);
            request.Prompt = context;
            await foreach (var chunk in selectedModel.AnalyzeAsync(request, cancellationToken))
            {
                var json = JsonSerializer.Serialize(new { content = chunk, done = false });
                await Response.WriteAsync($"data: {json}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }

            if (!cancellationToken.IsCancellationRequested)
            {
                var doneJson = JsonSerializer.Serialize(new { content = "", done = true });
                await Response.WriteAsync($"data: {doneJson}\n\n", cancellationToken);
                await Response.Body.FlushAsync(cancellationToken);
            }
        }
        catch (OperationCanceledException)
        {
            // Client disconnected - this is normal, just log it
            Console.WriteLine("Client cancelled the request");
        }
        catch (Exception ex)
        {
            // Handle other exceptions
            Console.WriteLine($"Error during streaming: {ex.Message}");
            throw;
        }
    }
}
