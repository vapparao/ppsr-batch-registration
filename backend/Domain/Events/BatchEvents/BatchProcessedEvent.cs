
using Domain.Entities;
using MediatR;

namespace Domain.Events.BatchEvents
{
    public class BatchProcessedEvent : INotification
    {
        public int ClientId { get; }
        public List<BatchOperation> CompletedBatches { get; }
        public List<BatchOperation> ProcessingBatches { get; }

        public BatchProcessedEvent(int clientId, List<BatchOperation> completedBatches, List<BatchOperation> processingBatches)
        {
            ClientId = clientId;
            CompletedBatches = completedBatches;
            ProcessingBatches = processingBatches;
        }
    }
}
