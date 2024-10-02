using Quartz;

namespace ProcessOutboxJob;

public class OutboxPublishJob(IOrderOutboxProcessService orderOutboxProcessService) : IJob
{
    private readonly IOrderOutboxProcessService _orderOutboxProcessService = orderOutboxProcessService;

    public async Task Execute(IJobExecutionContext context)
    {
        await _orderOutboxProcessService.ProcessRecordAsync();
    }
}
