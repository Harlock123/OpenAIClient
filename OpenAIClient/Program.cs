// See https://aka.ms/new-console-template for more information

public class Program
{
    public static async Task Main()
    {
        var client = new OllamaClient();
        Console.WriteLine("Generating story...");
        var story = await client.GenerateStoryAsync();
        Console.WriteLine("\nGenerated Story:");
        Console.WriteLine(story);
    }
}