using CJTasksHelperBot.Application.Task.Commands;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CJTasksHelperBot.Infrastructure.Services.BackgroundServices;

public class TaskStatusDeadlineMissedSetterService : BackgroundService
{
    private readonly TimeSpan _period = TimeSpan.FromHours(8);
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SoonExpiredTasksNotifierService> _logger;

    public TaskStatusDeadlineMissedSetterService(IServiceProvider serviceProvider, ILogger<SoonExpiredTasksNotifierService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(_period);
        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                await using var asyncScope = _serviceProvider.CreateAsyncScope();
                var mediator = asyncScope.ServiceProvider.GetRequiredService<IMediator>();
                await mediator.Send(new SetDeadlineMissedStatusCommand(), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError("Failed to execute {serviceName} with exception message: {message}", GetType(), ex.Message);
            }
        }
    }
}