using System;
using System.Data.Common;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Wrapperizer.Domain.Abstractions;
using Wrapperizer.Repository.EntityFrameworkCore.Abstraction;

namespace Wrapperizer.Outbox.Services.Internal
{
    public sealed class TransactionalOutboxService : ITransactionalOutboxService
    {
        private readonly ITransactionalUnitOfWork _unitOfWork;
        private readonly IPublishEndpoint _publishEndpoint;


        private readonly ILogger<TransactionalOutboxService> _logger;
        private readonly IOutboxEventService _outboxEventService;

        public TransactionalOutboxService(
            ITransactionalUnitOfWork unitOfWork,
            IOutboxEventService outboxEventService,
            IPublishEndpoint publishEndpoint,
            ILogger<TransactionalOutboxService> logger
        )
        {
            _unitOfWork = unitOfWork;
            _publishEndpoint = publishEndpoint;

            _logger = logger;
            
            // Todo: passing only the connection might be problematic, since in this version
            //       we are only using SqlServer, what if we want to have Postgres for instance?
            // _outboxEventService = integrationServiceFactory(unitOfWork.GetDbConnection());
            _outboxEventService = outboxEventService;
        }

        public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            var pendingLogEvents = await _outboxEventService.RetrievePendingEventsToPublishAsync(transactionId);

            foreach (var logEvt in pendingLogEvents)
            {
                _logger.LogInformation(
                    "----- Publishing integration event: {IntegrationEventId} - ({@IntegrationEvent})",
                    logEvt.EventId, logEvt.IntegrationEvent);

                try
                {
                    await _outboxEventService.MarkEventAsInProgressAsync(logEvt.EventId);

                    await _publishEndpoint.Publish(logEvt.IntegrationEvent, logEvt.IntegrationEvent.GetType());

                    await _outboxEventService.MarkEventAsPublishedAsync(logEvt.EventId);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "ERROR publishing integration event: {IntegrationEventId}",
                        logEvt.EventId);

                    await _outboxEventService.MarkEventAsFailedAsync(logEvt.EventId);
                }
            }
        }

        public async Task AddAndSaveEventAsync(IntegrationEvent @event)
        {
            _logger.LogInformation(
                "----- Enqueuing integration event {IntegrationEventId} to Outbox ({@IntegrationEvent})", @event.Id,
                @event);

            await _outboxEventService.SaveEventAsync(@event, _unitOfWork.GetCurrentTransaction());
        }
    }
}
