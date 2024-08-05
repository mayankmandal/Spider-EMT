using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spider_EMT.Configuration.IService;
using Spider_EMT.Models;
using Spider_EMT.Utility;
using System.Net.Http.Headers;
using System.Text;

namespace Spider_EMT.Configuration.Service
{
    public class UniquenessCheckService : IUniquenessCheckService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public string JwtToken { get; private set; }
        public UniquenessCheckService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IHttpContextAccessor httpContextAccessor) 
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> IsUniqueAsync(string field, string value)
        {
            JwtToken = JWTCookieHelper.GetJWTCookie(_httpContextAccessor.HttpContext);
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", JwtToken);
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
