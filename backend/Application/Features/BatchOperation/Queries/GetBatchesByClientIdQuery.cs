using Application.Features.BatchOperation.Dtos;
using AutoMapper;
using Domain.Enums;
using Domain.Interfaces;
using MediatR;

namespace Application.Features.BatchOperation.Queries
{
    public class GetBatchesByClientIdQuery : IRequest<List<Domain.Entities.BatchOperation>>
    {
        public int ClientId { get; set; }
        public required string Status { get; set; }
    }
    public class GetBatchesByClientIdQueryHandler : IRequestHandler<GetBatchesByClientIdQuery, List<Domain.Entities.BatchOperation>?>
    {
        private readonly IRepository<Domain.Entities.BatchOperation> _repository;
        private readonly IMapper _mapper;

        public GetBatchesByClientIdQueryHandler(
            IRepository<Domain.Entities.BatchOperation> repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<Domain.Entities.BatchOperation>?> Handle(
            GetBatchesByClientIdQuery request,
            CancellationToken cancellationToken)
        {
            var batches = await _repository.FindAsync((b => b.ClientId == request.ClientId && b.Status == (BatchStatus)Enum.Parse(typeof(BatchStatus), request.Status)));
            
            return batches.ToList();
        }


    }

}
