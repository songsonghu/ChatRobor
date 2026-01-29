namespace ChatRobor.Services
{
    public interface IDeepSeekService
    {
        Task<string> SendMessageAsync(string message, float temperature, int maxTokens, List<(string role, string content)> history);
    }

    public class DeepSeekService : IDeepSeekService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DeepSeekService> _logger;

        public DeepSeekService(HttpClient httpClient, IConfiguration configuration, ILogger<DeepSeekService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> SendMessageAsync(string message, float temperature, int maxTokens, List<(string role, string content)> history)
        {
            try
            {
                var apiKey = _configuration["DeepSeek:ApiKey"];
                if (string.IsNullOrEmpty(apiKey))
                {
                    throw new InvalidOperationException("DeepSeek API Key is not configured.");
                }

                var messages = new List<object>();

                // Add conversation history
                foreach (var (role, content) in history)
                {
                    messages.Add(new { role, content });
                }

                // Add current message
                messages.Add(new { role = "user", content = message });

                var requestBody = new
                {
                    model = _configuration["DeepSeek:Model"] ?? "deepseek-chat",
                    messages,
                    temperature,
                    max_tokens = maxTokens,
                    stream = false
                };

                var request = new HttpRequestMessage(HttpMethod.Post, _configuration["DeepSeek:ApiUrl"] ?? "https://api.deepseek.com/chat/completions")
                {
                    Content = new StringContent(
                        System.Text.Json.JsonSerializer.Serialize(requestBody),
                        System.Text.Encoding.UTF8,
                        "application/json"
                    )
                };

                request.Headers.Add("Authorization", $"Bearer {apiKey}");

                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"DeepSeek API error: {response.StatusCode} - {responseContent}");
                    throw new HttpRequestException($"DeepSeek API returned {response.StatusCode}");
                }

                var jsonResponse = System.Text.Json.JsonDocument.Parse(responseContent);
                var choices = jsonResponse.RootElement.GetProperty("choices");
                if (choices.GetArrayLength() > 0)
                {
                    var firstChoice = choices[0];
                    var assistantMessage = firstChoice.GetProperty("message").GetProperty("content").GetString();
                    return assistantMessage ?? "Unable to get response from DeepSeek API";
                }

                return "No response from DeepSeek API";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calling DeepSeek API");
                throw;
            }
        }
    }
}
