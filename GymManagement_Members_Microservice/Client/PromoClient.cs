using GymManagement_Shared_Classes.DTO_s;

namespace GymManagement_Members_Microservice.Client
{
    public class PromoClient(HttpClient http)
    {
        public async Task<PromoAnswerDTO> GetDiscountByCodeAsync(string code, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(code)) return null;

            using var resp = await http.GetAsync($"/api/promo/by-code/{Uri.EscapeDataString(code)}", ct);

            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;
            resp.EnsureSuccessStatusCode();

            return await resp.Content.ReadFromJsonAsync<PromoAnswerDTO>(cancellationToken: ct);
        }
    }
}
