using Newtonsoft.Json;
using Spider_EMT.Configuration.IService;
using Spider_EMT.Models;
using System.Text;

namespace Spider_EMT.Configuration.Service
{
    public class UniquenessCheckService : IUniquenessCheckService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public UniquenessCheckService(IHttpClientFactory httpClientFactory, IConfiguration configuration) 
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }
        public async Task<bool> IsUniqueAsync(string field, string value)
        {
            var client = _httpClientFactory.CreateClient();
            var requestPayload = new UniquenessCheckRequest { Field = field, Value = value };
            var content = new StringContent(JsonConvert.SerializeObject(requestPayload), Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{_configuration["ApiBaseUrl"]}/Navigation/CheckUniqueness", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiResponse>(responseContent);
            return result.IsUnique;
        }
        private class ApiResponse
        {
            public bool IsUnique;
        }
    }
}
