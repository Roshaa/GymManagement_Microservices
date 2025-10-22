using GymManagement_Shared_Classes.DTO_s;
using System.Net;
using System.Net.Http.Headers;

namespace GymManagement_MemberShips_Microservice.Client
{
    public class MemberDiscountClient(HttpClient http)
    {
        public async Task<MemberDiscountDTO?> GetMemberDiscountAsync(int memberId, CancellationToken ct = default)
        {
            if (memberId <= 0) return null;

            //Hammer cause personal project, i know perfectly this is not suited and i would refactor this in a real project:
            string hardcodedJwtToken = Environment.GetEnvironmentVariable("HardcodedJwt");
            //

            var req = new HttpRequestMessage(HttpMethod.Get, $"/api/member/{memberId}/discount");
            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", hardcodedJwtToken);

            using var resp = await http.SendAsync(req, ct);

            if (resp.StatusCode == HttpStatusCode.NotFound) return null;

            resp.EnsureSuccessStatusCode();

            return await resp.Content.ReadFromJsonAsync<MemberDiscountDTO>(cancellationToken: ct);

        }
    }
}