namespace API_QLKHACHSAN.Service
{
    public class LoaiPhongServices
    {
        public async Task<decimal> TinhGiaLoaiPhong(int maLoaiPhong, bool phiThemNguoi)
        {
            using (var _httpClient = new HttpClient())
            {
                var url = $"http://localhost:5149/api/LoaiPhong/TinhGiaLoaiPhong?maLoaiPhong={maLoaiPhong}&coThemNguoi={phiThemNguoi}";
                var response = await _httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<decimal>();
                }
                throw new Exception($"Error: {response.StatusCode}");
            }
        }
    }
}
