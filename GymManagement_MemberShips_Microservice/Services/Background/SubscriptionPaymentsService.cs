using Azure.Messaging.ServiceBus;
using GymManagement_MemberShips_Microservice.Client;
using GymManagement_MemberShips_Microservice.Config;
using GymManagement_MemberShips_Microservice.Context;
using GymManagement_MemberShips_Microservice.Models;
using GymManagement_Shared_Classes.DTO_s;
using GymManagement_Shared_Classes.Events;
using Microsoft.EntityFrameworkCore;

namespace GymManagement_MemberShips_Microservice.Services.Background
{
    public sealed class SubscriptionPaymentsService : BackgroundService
    {
        private readonly ILogger<SubscriptionPaymentsService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ServiceBusSender _sender;
        private readonly MemberDiscountClient _memberDiscountClient;

        public SubscriptionPaymentsService(
            ILogger<SubscriptionPaymentsService> logger,
            IServiceScopeFactory scopeFactory,
            ServiceBusSender sender,
            MemberDiscountClient memberDiscountClient)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _sender = sender;
            _memberDiscountClient = memberDiscountClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("SubscriptionPaymentsService started.");

            // SCHEDULE TO RUN DAILY AT 06:00 (LOCAL TIME) — commented as requested:
            // var nextRun = NextRunAtHour(6);
            // var initialDelay = nextRun - DateTimeOffset.Now;
            // _logger.LogInformation("Next scheduled run at {NextRun}. Initial delay: {Delay}", nextRun, initialDelay);
            // await Task.Delay(initialDelay, stoppingToken);

            // RUN ON APP STARTUP:
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

            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            MemberSubscription[] dueSubscriptions = await db.MemberSubscriptions
                .Where(ms => ms.DebitActive && ms.PaymentDay <= today)
                .ToArrayAsync(ct);

            if (dueSubscriptions.Length > 0)
            {
                _logger.LogInformation("Processing {Count} due subscription payments for {Date}.", dueSubscriptions.Length, today);

                decimal baseSubscriptionPrice = PaymentConfig.MembershipFee;
                var messages = new List<ServiceBusMessage>();

                foreach (MemberSubscription ms in dueSubscriptions)
                {
                    //Simulate a 10% chance of payment failure due to cancelled direct debit in bank account
                    int changeUserCancelledInBankAccount = new Random().Next(1, 101);

                    if (changeUserCancelledInBankAccount <= 10)
                    {
                        _logger.LogWarning("Simulating payment failure for MemberSubscription with Id {Id} due to cancelled direct debit in bank account.", ms.MemberId);

                        PaymentMessage debitCancelledMessage = new PaymentMessage(ms.MemberId, false, null);

                        var msgbody = BinaryData.FromObjectAsJson(debitCancelledMessage);
                        ms.DebitActive = false;

                        var cancelledDebitMsg = new ServiceBusMessage(msgbody)
                        {
                            ContentType = "application/json",
                            Subject = "SubscriptionPayment",
                            MessageId = $"pay:{ms.MemberId}:{today:yyyyMMdd}",
                            CorrelationId = ms.MemberId.ToString()
                        };

                        messages.Add(cancelledDebitMsg);

                        continue;
                    }

                    MemberDiscountDTO discount = await _memberDiscountClient.GetMemberDiscountAsync(ms.MemberId, ct);
                    decimal gymFee = discount != null ? baseSubscriptionPrice - (baseSubscriptionPrice * discount.Discount) : baseSubscriptionPrice;

                    PaymentHistory payment = new PaymentHistory
                    {
                        MemberId = ms.MemberId,
                        IBAN = ms.IBAN,
                        PaymentDate = today,
                        Amount = gymFee
                    };

                    db.PaymentHistory.Add(payment);
                    ms.PaymentDay = ms.PaymentDay.AddMonths(1);

                    PaymentMessage paymentMessage = new PaymentMessage(ms.MemberId, true, ms.PaymentDay);

                    var body = BinaryData.FromObjectAsJson(paymentMessage);

                    var msg = new ServiceBusMessage(body)
                    {
                        ContentType = "application/json",
                        Subject = "SubscriptionPayment",
                        MessageId = $"pay:{ms.MemberId}:{today:yyyyMMdd}",
                        CorrelationId = ms.MemberId.ToString()
                    };

                    messages.Add(msg);
                }

                await db.SaveChangesAsync(ct);
                await SendInBatchesAsync(_sender, messages, ct);

                _logger.LogInformation("Published {Count} messages and saved payments.", messages.Count);
            }
        }

        private async Task SendInBatchesAsync(ServiceBusSender sender, IEnumerable<ServiceBusMessage> messages, CancellationToken ct)
        {
            ServiceBusMessageBatch batch = await sender.CreateMessageBatchAsync(ct);

            try
            {
                foreach (var msg in messages)
                {
                    if (!batch.TryAddMessage(msg))
                    {
                        await sender.SendMessagesAsync(batch, ct);
                        batch.Dispose();

                        batch = await sender.CreateMessageBatchAsync(ct);

                        if (!batch.TryAddMessage(msg)) throw new InvalidOperationException("Single message too large for an empty batch.");
                    }
                }

                if (batch.Count > 0) await sender.SendMessagesAsync(batch, ct);
            }
            finally
            {
                batch.Dispose();
            }

        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("SubscriptionPaymentsService stopping.");
            return base.StopAsync(cancellationToken);
        }

    }
}
