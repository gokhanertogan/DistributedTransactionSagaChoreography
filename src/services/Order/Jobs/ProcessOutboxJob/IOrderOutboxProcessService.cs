using System.Text.Json;
using MassTransit;
using OrderService.Application.Repositories;
using OrderService.Domain.Entities;
using Shared.Events;

namespace ProcessOutboxJob;

public interface IOrderOutboxProcessService
{
    Task<bool> ProcessRecordAsync();
}

public class OrderOutboxProcessService(IOrderOutboxRepository orderOutboxRepository, IPublishEndpoint publishEndpoint) : IOrderOutboxProcessService
{
    private readonly IOrderOutboxRepository _orderOutboxRepository = orderOutboxRepository;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private readonly Guid _workerGuid = Guid.NewGuid();

    #region PrivateMethods
    private List<OrderOutbox> GetPendingRecords()
    {
        return [.. _orderOutboxRepository.GetWhere(x => x.Status == OrderService.Domain.Enums.OrderOutboxStatus.Pending).OrderBy(x => x.OccurredOn).Take(10)];
    }

    private async Task<bool> MarkAsInProgressAsync(Guid idempotentToken, int currentVersion)
    {
        var record = _orderOutboxRepository.GetWhere(x => x.IdempotentToken == idempotentToken && x.Version == currentVersion).FirstOrDefault();

        if (record != null)
        {
            record.Status = OrderService.Domain.Enums.OrderOutboxStatus.InProgress;
            record.ProcessedBy = _workerGuid;
            record.Version++;

            _orderOutboxRepository.Update(record);
            await _orderOutboxRepository.SaveChangesAsync();

            return true;
        }

        return false;
    }

    private async Task<bool> MarkAsProcessedAsync(Guid idempotentToken, int currentVersion)
    {
        var record = _orderOutboxRepository.GetWhere(x => x.IdempotentToken == idempotentToken && x.Version == currentVersion).FirstOrDefault();

        if (record != null)
        {
            record.ProcessedDate = DateTime.UtcNow;
            record.Status = OrderService.Domain.Enums.OrderOutboxStatus.Processed;
            record.Version++;

            _orderOutboxRepository.Update(record);
            await _orderOutboxRepository.SaveChangesAsync();

            return true;
        }

        return false;
    }

    #endregion


    public async Task<bool> ProcessRecordAsync()
    {
        try
        {
            if (OrderSingletonDatabase.DataReaderState)
            {
                OrderSingletonDatabase.DataReaderBusy();
                var pendingRecords = GetPendingRecords();

                foreach (var record in pendingRecords)
                {
                    await MarkAsInProgressAsync(record.IdempotentToken, record.Version);

                    if (record.Type == nameof(OrderCreatedEvent))
                    {
                        var order = JsonSerializer.Deserialize<Order>(record.Payload!);
                        if (order != null)
                        {
                            OrderCreatedEvent orderCreatedEvent = new()
                            {
                                Description = order.Description,
                                OrderId = order.Id,
                                Quantity = order.Quantity,
                                IdempotentToken = record.IdempotentToken
                            };

                            await _publishEndpoint.Publish(orderCreatedEvent);
                        }
                    }

                    await MarkAsProcessedAsync(record.IdempotentToken, record.Version);
                }

                OrderSingletonDatabase.DataReaderReady();
            }

            return true;
        }
        catch
        {
            return false;
        }
    }
}
