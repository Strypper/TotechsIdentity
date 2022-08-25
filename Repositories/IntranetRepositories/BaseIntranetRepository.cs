using Contracts.Intranet;
using Entities;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.IntranetRepositories
{
    public class BaseIntranetRepository<T> : IBaseIntranetRepository<T> where T : BaseEntity
    {
        private readonly HttpClient _httpClient;

        public BaseIntranetRepository(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _httpClient.BaseAddress = new Uri(IntranetConstants.BaseUrl);
            
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            var response = await _httpClient.GetAsync(url);
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(result);
        }

        public async Task<T?> GetByIdAsync<T>(string url, int id)
        {
            string finalGetByIdUrl(int entityId) => $"{url}/{entityId}";
            var response = await _httpClient.GetAsync(finalGetByIdUrl(id));
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(result);
        }

        public async Task<T?> CreateAsync<T>(string url, object o)
        {
            var content = new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(result);
        }

        public async Task<bool> CreateAsyncWithoutDTO<T>(string url, object o)
        {
            var content = new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(url, content);
            return response.StatusCode == HttpStatusCode.OK ? true : false;
        }

        public async Task<bool> UpdateAsync(string url, object o)
        {
            var content = new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(url, content);
            return response.StatusCode == HttpStatusCode.NoContent ? true : false;
        }

        public async Task<bool> RemoveAsync(string url, int id)
        {
            string finalDeleteUrl(int entityId) => $"{url}/{entityId}";
            var response = await _httpClient.DeleteAsync(finalDeleteUrl(id));
            return response.StatusCode == HttpStatusCode.NoContent ? true : false;
        }
    }
}