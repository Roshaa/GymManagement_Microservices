using Azure.Messaging.ServiceBus;
using GymManagement_Members_Microservice.Context;
using GymManagement_Members_Microservice.Models;
using GymManagement_Shared_Classes.Events; // PaymentMessage record
using Microsoft.EntityFrameworkCore;

namespace GymManagement_Members_Microservice.Services.Background;

public sealed class SubscriptionPaymentsConsumerService : IHostedService
{
    private readonly ServiceBusProcessor _processor;
    private readonly ILogger<SubscriptionPaymentsConsumerService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public SubscriptionPaymentsConsumerService(
        ServiceBusProcessor processor,
        ILogger<SubscriptionPaymentsConsumerService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _processor = processor;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    public async Task StartAsync(CancellationToken ct)
    {
        _processor.ProcessMessageAsync += HandleMessageAsync;
        _processor.ProcessErrorAsync += HandleErrorAsync;
        await _processor.StartProcessingAsync(ct);

        _logger.LogInformation("SubscriptionPaymentsConsumerService started.");
    }

    public async Task StopAsync(CancellationToken ct)
    {
        await _processor.StopProcessingAsync(ct);
        await _processor.DisposeAsync();
        _logger.LogInformation("SubscriptionPaymentsConsumerService stopped.");
    }

    private async Task HandleMessageAsync(ProcessMessageEventArgs args)
    {
        PaymentMessage paymentMessage;

        try
        {
            paymentMessage = args.Message.Body.ToObjectFromJson<PaymentMessage>();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Bad payload. Dead-lettering. MessageId={MessageId}", args.Message.MessageId);
            await args.DeadLetterMessageAsync(args.Message, "Bad payload", ex.Message);
            return;
        }

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        Member member = await db.Member.SingleOrDefaultAsync(m => m.Id == paymentMessage.MemberId, args.CancellationToken);

        if (member is null)
        {
            _logger.LogWarning("Member not found. Dead-lettering. MemberId={MemberId}", paymentMessage.MemberId);
            await args.DeadLetterMessageAsync(args.Message, "Member not found");
            return;
        }

        try
        {
            if (paymentMessage.PaymentSuccessful)
            {
                member.ActiveUntilDay = paymentMessage.NextPayment!.Value;
                _logger.LogInformation("Payment OK for MemberId={MemberId}. Next={Next}", paymentMessage.MemberId, paymentMessage.NextPayment);
            }
            else
            {
                //Send a notification...
                _logger.LogWarning("Payment FAILED for MemberId={MemberId}. Debit disabled.", paymentMessage.MemberId);
            }

            await db.SaveChangesAsync(args.CancellationToken);
            await args.CompleteMessageAsync(args.Message);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "DB error. Abandoning for retry. MessageId={MessageId}", args.Message.MessageId);
            await args.AbandonMessageAsync(args.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error. Dead-lettering. MessageId={MessageId}", args.Message.MessageId);
            await args.DeadLetterMessageAsync(args.Message, "Unhandled processing error", ex.Message);
        }
    }

    private Task HandleErrorAsync(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "ServiceBus error. Entity={Entity} Source={Source}", args.EntityPath, args.ErrorSource);
        return Task.CompletedTask;
    }

}
