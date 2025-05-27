using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Services.SignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Services.SignalR
{
    public class BatchHubService : INotificationPublisher
    {
        private readonly IHubContext<BatchHub> _hubContext;

        public BatchHubService(IHubContext<BatchHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task PublishBatchProcessingCompleted(int clientId, List<BatchOperation> completedBatches, List<BatchOperation> processingBatches)
        {
            await _hubContext.Clients.All.SendAsync("BatchProcessed", clientId, completedBatches, processingBatches);
        }
    }
}
