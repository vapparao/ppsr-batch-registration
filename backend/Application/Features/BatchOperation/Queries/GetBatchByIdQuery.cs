using Application.Features.BatchOperation.Dtos;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.BatchOperation.Queries
{
    public class GetBatchByIdQuery : IRequest<BatchDto>
    {
        public int BatchId { get; set; }
    }
    public class GetBatchByIdQueryHandler : IRequestHandler<GetBatchByIdQuery, BatchDto?>
    {
        private readonly IRepository<Domain.Entities.BatchOperation> _repository;
        private readonly IMapper _mapper;

        public GetBatchByIdQueryHandler(
            IRepository<Domain.Entities.BatchOperation> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<BatchDto?> Handle(
            GetBatchByIdQuery request,
            CancellationToken cancellationToken)
        {
            var batch = await _repository.GetByIdAsync(request.BatchId);

            return _mapper.Map<BatchDto>(batch);
        }


    }
    
}
