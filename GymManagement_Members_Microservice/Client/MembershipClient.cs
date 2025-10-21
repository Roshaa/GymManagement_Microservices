using GymManagement_Members_Microservice.DTO_s;

namespace GymManagement_Members_Microservice.Client
{
    public class MemberShipClient(HttpClient http)
    {
        public async Task<bool> CreateSubscriptionPayment(CreateMemberSubscriptionDTO dto, CancellationToken ct = default)
        {
            if (dto is null) return false;

            using var resp = await http.PostAsJsonAsync("/api/memberships", dto, ct);

            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

            return resp.IsSuccessStatusCode;
        }

        public async Task<bool> ChangeMemberDebitStatus(ChangeMemberSubscriptionDTO dto, CancellationToken ct = default)
        {
            if (dto is null) return false;

            using var resp = await http.PutAsJsonAsync("/api/memberships/change_debit", dto, ct);

            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return false;

            return resp.IsSuccessStatusCode;
        }
    }
}
