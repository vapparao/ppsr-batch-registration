using Domain.Events.BatchEvents;
using MediatR;

namespace Infrastructure.Services.Notifications
{
    public class NotificationDispatcher :
    INotificationHandler<BatchProcessedEvent>
    {
        private readonly Domain.Interfaces.INotificationPublisher _publisher;

        public NotificationDispatcher(Domain.Interfaces.INotificationPublisher publisher)
        {
            _publisher = publisher;
        }

        public async Task Handle(BatchProcessedEvent notification, CancellationToken cancellationToken)
        {
            await _publisher.PublishBatchProcessingCompleted(
                notification.ClientId, notification.CompletedBatches, notification.ProcessingBatches);
        }
    }
}
