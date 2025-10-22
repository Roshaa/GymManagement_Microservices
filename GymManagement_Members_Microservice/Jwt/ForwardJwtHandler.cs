namespace GymManagement_Members_Microservice.Jwt
{
    public class ForwardJwtHandler(IHttpContextAccessor acc) : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage req, CancellationToken ct)
        {
            var auth = acc.HttpContext?.Request.Headers["Authorization"].ToString();

            if (!string.IsNullOrWhiteSpace(auth) && !req.Headers.Contains("Authorization"))
                req.Headers.TryAddWithoutValidation("Authorization", auth);

            return base.SendAsync(req, ct);
        }
    }
}
