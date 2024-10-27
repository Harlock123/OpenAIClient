using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;  // Add this for JsonPropertyName

public class OllamaRequest
{
    public string Model { get; set; }
    public string Prompt { get; set; }
}

public class OllamaResponse
{
    [JsonPropertyName("response")]  // This maps to the lowercase "response" in the JSON
    public string Response { get; set; }

    [JsonPropertyName("done")]      // This maps to the lowercase "done" in the JSON
    public bool Done { get; set; }

    [JsonPropertyName("model")]     // Optional: if you need other fields
    public string Model { get; set; }

    [JsonPropertyName("created_at")]
    public string CreatedAt { get; set; }
}

public class OllamaClient
{
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    public OllamaClient(string baseUrl = "http://localhost:11434")
    {
        _baseUrl = baseUrl;
        _httpClient = new HttpClient();
    }

    public async Task<string> GenerateStoryAsync()
    {
        var request = new OllamaRequest
        {
            Model = "codellama:34b",
            Prompt = "Write a 1000 word story. Make it creative and engaging. Include proper story elements like introduction, conflict, and resolution."
        };

        var jsonContent = JsonSerializer.Serialize(request);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        StringBuilder storyBuilder = new StringBuilder();

        try
        {
            var response = await _httpClient.PostAsync($"{_baseUrl}/api/generate", content);
            response.EnsureSuccessStatusCode();

            using var reader = new StreamReader(await response.Content.ReadAsStreamAsync());
            string line;

            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (string.IsNullOrEmpty(line)) continue;

                var streamResponse = JsonSerializer.Deserialize<OllamaResponse>(line);
                if (streamResponse != null)
                {
                    storyBuilder.Append(streamResponse.Response);
                    
                    if (streamResponse.Done)
                        break;
                }
            }

            // Clean up the story text
            var result = storyBuilder.ToString()
                .Replace("\n\n", "\n")  // Remove double line breaks
                .Trim();                // Remove leading/trailing whitespace

            return result;
        }
        catch (Exception ex)
        {
            return $"Error generating story: {ex.Message}";
        }
    }
}