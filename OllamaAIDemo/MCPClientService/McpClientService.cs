using ModelContextProtocol.Client;

namespace OllamaAIDemo.MCPClientService;

public interface IMcpClientService
{
    Task<McpClient> GetClientAsync();
}

public class DefaultMcpClientService : IMcpClientService
{
    private McpClient? _client;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public async Task<McpClient> GetClientAsync()
    {
        if (_client != null) return _client;

        await _semaphore.WaitAsync();
        try
        {
            // Double-check locking pattern to ensure we only connect once
            if (_client == null)
            {
                var transport = new StdioClientTransport(new StdioClientTransportOptions
                {
                    Command = "cmd",
                    Arguments = new[] { "/c", "D:\\My Project\\MCPDemo\\MCPServer.ExpenseIncome\\bin\\Release\\net10.0\\MCPServer.ExpenseIncome.exe" }
                });

                _client = await McpClient.CreateAsync(transport);
            }
            return _client;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}
