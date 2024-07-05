using Newtonsoft.Json;
using Spider_EMT.Configuration.IService;

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
            var response = await client.GetStringAsync($"{_configuration["ApiBaseUrl"]}/Navigation/CheckUniqueness?field={field}&value={value}");
            var result = JsonConvert.DeserializeObject<ApiResponse>(response);
            return result.IsUnique;
        }
        private class ApiResponse
        {
            public bool IsUnique;
        }
    }
}
