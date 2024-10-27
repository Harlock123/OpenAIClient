using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

//namespace OpenAIClient;

public class OpenAIClient
{
    private readonly HttpClient _httpClient;
    private readonly string _endpointUrl;

    public OpenAIClient(string endpointUrl)
    {
        _httpClient = new HttpClient();
        _endpointUrl = endpointUrl;
    }

    public async Task<string> SendPromptAsync(string prompt)
    {
        var requestData = new
        {
            prompt = prompt
        };

        var content = new StringContent(JsonSerializer.Serialize(requestData), Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.PostAsync(_endpointUrl, content);
        response.EnsureSuccessStatusCode();

        string responseContent = await response.Content.ReadAsStringAsync();
        var responseData = JsonSerializer.Deserialize<OpenAIResponse>(responseContent);

        return responseData?.Result;
    }
}

public class OpenAIResponse
{
    public string Result { get; set; }
}