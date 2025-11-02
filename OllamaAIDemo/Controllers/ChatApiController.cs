using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OllamaAIDemo.ChatClient;

namespace OllamaAIDemo.Controllers;

[Route("api/chats")]
[ApiController]
public class ChatApiController : ControllerBase
{
    private readonly IApplicationChatClient _applicationChatClient;

    public ChatApiController(IApplicationChatClient applicationChatClient)
    {
        this._applicationChatClient = applicationChatClient;
    }

    [HttpPost]
    public async Task<IActionResult> ChatAsync(string prompt, CancellationToken cancellationToken)
    {
        var response = await _applicationChatClient.ChatAsync(prompt, cancellationToken);
        return Ok(response);
    }
}
