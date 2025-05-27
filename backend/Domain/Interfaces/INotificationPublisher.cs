using Domain.Entities;

namespace Domain.Interfaces
{
    public interface INotificationPublisher
    {
        Task PublishBatchProcessingCompleted(int clientId, List<BatchOperation> completedBatches, List<BatchOperation> processingBatches);
    }
}
