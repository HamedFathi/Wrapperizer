using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Sample.IntegrationEvents;
using Wrapperizer.Abstraction.Domain;
using Wrapperizer.Outbox.Services;
using Wrapperizer.Sample.Domain.Events;

namespace Sample.Api
{
    public sealed class StudentRegisteredHandler : IDomainEventHandler<StudentRegistered>
    {
        private readonly ILogger<StudentRegisteredHandler> _logger;
        private readonly IIntegrationService _integrationService;

        public StudentRegisteredHandler(ILogger<StudentRegisteredHandler> logger,
            IIntegrationService integrationService)
        {
            _logger = logger;
            _integrationService = integrationService;
        }
        
        public async Task Handle(StudentRegistered notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Event {EventName} handled by {HandlerName} for {@Event}" ,
                notification.GetType().Name, this.GetType().Name, notification
                );
            
            var integrationEvent = new StudentRegisteredIntegrationEvent(notification.Student.Id, 
                $"{notification.Student.FirstName} {notification.Student.LastName}");
            
            await _integrationService.AddAndSaveEventAsync(integrationEvent);
            
        }
    }
}
