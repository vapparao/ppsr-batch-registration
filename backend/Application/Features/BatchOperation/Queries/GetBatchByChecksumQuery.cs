using Application.Features.BatchOperation.Dtos;
using AutoMapper;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.BatchOperation.Queries
{
    public class GetBatchByChecksumQuery : IRequest<BatchDto>
    {
        public int ClientId { get; set; }
        public required string FileChecksum { get; set; }
    }
    public class GetBatchByChecksumQueryHandler : IRequestHandler<GetBatchByChecksumQuery, BatchDto?>
    {
        private readonly IRepository<Domain.Entities.BatchOperation> _repository;
        private readonly IMapper _mapper;

        public GetBatchByChecksumQueryHandler(
            IRepository<Domain.Entities.BatchOperation> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<BatchDto?> Handle(
            GetBatchByChecksumQuery request,
            CancellationToken cancellationToken)
        {
            var batches = await _repository.FindAsync(b => b.FileChecksum == request.FileChecksum && b.ClientId == request.ClientId);
            var batchDto = batches.Any()? _mapper.Map<BatchDto>(batches.First()) : null;

            return batchDto;
        }


    }
    
}
