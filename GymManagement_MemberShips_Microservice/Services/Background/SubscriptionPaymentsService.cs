using GymManagement_MemberShips_Microservice.Context;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_MemberShips_Microservice.Services.Background
{
    public sealed class SubscriptionPaymentsService : BackgroundService
    {
        private readonly ILogger<SubscriptionPaymentsService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;

        public SubscriptionPaymentsService(
            ILogger<SubscriptionPaymentsService> logger,
            IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SubscriptionPaymentsService started.");

            // SCHEDULE TO RUN DAILY AT 06:00 (LOCAL TIME) — commented as requested:
            // var nextRun = NextRunAtHour(6);
            // var initialDelay = nextRun - DateTimeOffset.Now;
            // _logger.LogInformation("Next scheduled run at {NextRun}. Initial delay: {Delay}", nextRun, initialDelay);
            // await Task.Delay(initialDelay, stoppingToken);

            // RUN ON APP STARTUP INSTEAD:
            await ProcessDueSubscriptionPaymentsAsync(stoppingToken);
        }

        private static DateTimeOffset NextRunAtHour(int hourLocal)
        {
            var now = DateTimeOffset.Now;
            var todayAtHour = new DateTimeOffset(now.Year, now.Month, now.Day, hourLocal, 0, 0, now.Offset);
            return now <= todayAtHour ? todayAtHour : todayAtHour.AddDays(1);
        }

        private async Task ProcessDueSubscriptionPaymentsAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            DateOnly today = DateOnly.FromDateTime(DateTime.Now); // local date to match local 06:00 scheduling

            var dueSubscriptions = await db.MemberSubscriptions
                .AsNoTracking()
                .Where(ms => ms.DebitActive && ms.PaymentDay == today)
                .ToListAsync(ct);

            _logger.LogInformation("Processing {Count} due subscription payments for {Date}.", dueSubscriptions.Count, today);

            //client for api request to see if it has discount...

            //save payments to cosmosdb

            //send to service bus

            //generate random chance of user cancelled direct debt in his bank account/wrong iban etc

            // TODO: Execute your payment logic here and persist any updates.
            // Example (if you track and update entities, remove AsNoTracking above):
            // foreach (var sub in dueTracked)
            // {
            //     // charge sub ...
            // }
            // await db.SaveChangesAsync(ct);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SubscriptionPaymentsService stopping.");
            return base.StopAsync(cancellationToken);
        }
    }
}
