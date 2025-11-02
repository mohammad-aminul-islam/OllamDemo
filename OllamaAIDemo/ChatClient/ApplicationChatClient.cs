using Microsoft.Extensions.AI;
using OllamaSharp;

namespace OllamaAIDemo.ChatClient;
public class ApplicationChatClient : IApplicationChatClient
{
    private readonly IChatClient _chatClient;

    public ApplicationChatClient(IChatClient chatClient)
    {
        this._chatClient = chatClient;
    }
    public async Task<string> ChatAsync(string prompt, CancellationToken cancellationToken)
    {
        try
        {
            List<ChatMessage> chatHistory = new List<ChatMessage>();
            var res = await _chatClient.GetResponseAsync(new ChatMessage(ChatRole.User, prompt));
            string responese = "";
            foreach (ChatMessage chatMessage in res.Messages)
            {
                responese += chatMessage.Text;
                chatHistory.Add(chatMessage);
            }
            return responese;
            //while (true)
            //{
            //    Console.WriteLine("What's on your mind! Mohammad");
            //    string prompt = Console.ReadLine();
            //    chatHistory.Add(new ChatMessage(ChatRole.User, prompt));
            //    string response = string.Empty;
            //    await foreach (var item in chatClient.GetStreamingResponseAsync(chatHistory, cancellationToken: cancellationToken))
            //    {
            //        Console.WriteLine(item.Text);
            //        response += item.Text;
            //    }
            //    chatHistory.Add(new ChatMessage(ChatRole.Assistant, response));
            //    Console.ReadLine();
            //}
        }
        catch (Exception ex)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine($"{ex.Message}\n {ex.StackTrace}");
            return ex.Message;
        }

    }
}
