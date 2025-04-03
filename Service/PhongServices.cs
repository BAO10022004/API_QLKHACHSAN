using API_QLKHACHSAN.Models;
using java.awt.print;
using javax.swing.text.html;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace API_QLKHACHSAN.Service
{
    public class PhongServices
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;

        public PhongServices()
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException("appsettings.json file not found in the application directory.");
                }
                var json = File.ReadAllText(filePath);
                var settings = JsonConvert.DeserializeObject<AppConfig>(json);
                _baseUrl = settings?.AppSettings?.BaseUrl ?? throw new Exception("BaseUrl is not configured in appsettings.json");
                _httpClient = new HttpClient();
                _httpClient.BaseAddress = new Uri(_baseUrl);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Session.Token);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error initializing AuthService: {ex.Message}", ex);
            }
        }
        public async Task<List<Phong>> GetListRoom()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_baseUrl}/api/Phong/GetRoom");

                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<Response>();
                return result.ListRoom ?? new List<Phong>();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tải danh sách phòng", ex);
            }
        }
        public async Task<List<Phong>> GetListRoomEmpty(DateOnly NgayNhan, DateOnly NgayTra)
        {
            using (var _httpClient = new HttpClient())
            {
                var url = $"http://localhost:5149/api/Phong/GetRoom?NgayNhan={NgayNhan}&NgayTra={NgayTra}";
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<Phong>>() ?? new List<Phong>();
                }
                throw new Exception("Lỗi khi tìm kiếm sách.");
            }
        }

        public class Response
        {
            public string Messesge { get; set; }
            public List<Phong> ListRoom { get; set; }
        }
    }
}
