using Application.Features.BatchOperation.Dtos;
using AutoMapper;
using Domain.Entities;

namespace Application.Mappings
{
    public class BatchProfile : Profile
    {
        public BatchProfile()
        {
            CreateMap<CreateBatchDto, BatchOperation>();
            CreateMap<BatchOperation, BatchDto>();
            CreateMap<BatchDto, BatchOperation>();

            CreateMap<List<BatchOperation>, List<BatchDto>>()
                .ConvertUsing((source, destination, context) =>
                    context.Mapper.Map<List<BatchDto>>(source));
        }
    }
}