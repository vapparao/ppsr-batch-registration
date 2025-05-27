using Application.Features.BatchOperation.Dtos;
using Application.Features.BatchOperation.Queries;
using AutoMapper;
using Domain.Enums;
using Domain.Events.BatchEvents;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.BatchOperation.Commands
{
    public class CreateBatchCommand : IRequest<BatchDto>
    {
        public required CreateBatchDto Batch { get; set; }
    }
    public class CreateBatchCommandHandler : IRequestHandler<CreateBatchCommand, BatchDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public CreateBatchCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<BatchDto> Handle(CreateBatchCommand request, CancellationToken cancellationToken)
        {
            var batch = _mapper.Map<Domain.Entities.BatchOperation>(request.Batch);
            batch.Status = BatchStatus.PROCESSING;
            batch.SubmittedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Domain.Entities.BatchOperation>().AddAsync(batch);
            await _unitOfWork.CompleteAsync();

            var publishResultsCompleted = await _mediator.Send(new GetBatchesByClientIdQuery
            {
                ClientId = batch.ClientId,
                Status = BatchStatus.COMPLETED.ToString(),
            });

            var publishResultsProgress = await _mediator.Send(new GetBatchesByClientIdQuery
            {
                ClientId = batch.ClientId,
                Status = BatchStatus.PROCESSING.ToString(),
            });

            await _mediator.Publish(new BatchProcessedEvent(batch.ClientId, publishResultsCompleted, publishResultsProgress), cancellationToken);

            return _mapper.Map<BatchDto>(batch);
        }
    }
}
