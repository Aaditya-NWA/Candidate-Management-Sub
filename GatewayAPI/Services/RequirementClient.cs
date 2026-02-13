using GatewayAPI.DTOs.Requirements;
using System.Net.Http.Json;

namespace GatewayAPI.Services
{
    public class RequirementClient
    {
        private readonly HttpClient _httpClient;

        public RequirementClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<HttpResponseMessage> CreateAsync(CreateRequirementDto dto)
        {
            return await _httpClient.PostAsJsonAsync(
                "/api/Requirements",
                dto);
        }

        public async Task<HttpResponseMessage> GetAllAsync()
        {
            return await _httpClient.GetAsync("/api/Requirements");
        }

        public async Task<HttpResponseMessage> GetByIdAsync(int id)
        {
            return await _httpClient.GetAsync($"/api/Requirements/{id}");
        }
    }
}
