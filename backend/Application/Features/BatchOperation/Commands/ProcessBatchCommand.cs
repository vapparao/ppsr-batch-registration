using Domain.Entities;
using Application.Features.BatchOperation.Dtos;
using System.Globalization;
using MediatR;
using AutoMapper;
using Domain.Interfaces;
using Application.Features.BatchOperation.Queries;
using Domain.Enums;
using Domain.Events.BatchEvents;
using Microsoft.Extensions.Logging;

namespace Application.Features.BatchOperation.Commands
{
    public class ProcessBatchCommand : IRequest<BatchDto>
    {
        public required CreateRegistrationsDto CsvData { get; set; }
    }

    public class ProcessBatchCommandHandler : IRequestHandler<ProcessBatchCommand, BatchDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILogger<ProcessBatchCommandHandler> _logger;
        public ProcessBatchCommandHandler(IUnitOfWork unitOfWork, IMapper mapper, IMediator mediator, ILogger<ProcessBatchCommandHandler> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
        }
        public async Task<BatchDto> Handle(ProcessBatchCommand request, CancellationToken cancellationToken)
        {
             var batch = await _unitOfWork.Repository<Domain.Entities.BatchOperation>()
                .GetByIdAsync(request.CsvData.BatchId);

            if (batch == null)
            {
                _logger.LogError("Batch with ID {BatchId} not found", request.CsvData.BatchId);
                throw new ArgumentException($"Batch with ID {request.CsvData.BatchId} not found");
            }

            if (request?.CsvData?.Rows == null)
            {
                throw new ArgumentNullException(nameof(request.CsvData), "CSV data cannot be null");
            }

            try
            {

                var validRows = request.CsvData.Rows.Where(x => x.IsValid).ToList();
                var invalidRows = request.CsvData.Rows.Where(x => !x.IsValid).ToList();

                batch.TotalRecords = request.CsvData.Rows.Count();
                batch.InvalidRecords = invalidRows.Count;

                if (invalidRows.Any())
                {
                    var invalidRecords = GetDistinctInvalidRecords(invalidRows, batch.BatchId);
                    await _unitOfWork.Repository<Domain.Entities.InvalidRecord>()
                        .AddRangeAsync(invalidRecords);
                }

                if (validRows.Any())
                {
                    int addedRecords = 0;
                    int updatedRecords = 0;

                    foreach (var record in validRows)
                    {
                        try
                        {
                           var existingGrantor = await _unitOfWork.Repository<Domain.Entities.Grantor>()
                                .FindAsync(g => g.FirstName == record.FirstName && g.LastName == record.LastName);
                            var grantor = existingGrantor.FirstOrDefault() ?? new Grantor
                            {
                                FirstName = record.FirstName ?? string.Empty,
                                MiddleNames = record.MiddleNames,
                                LastName = record.LastName ?? string.Empty,
                            };

                            var existingSpg = await _unitOfWork.Repository<Domain.Entities.Spg>()
                                .FindAsync(s => s.Acn == record.Acn);
                            var spg = existingSpg.FirstOrDefault() ?? new Spg
                            {
                                Acn = record.Acn ?? string.Empty,
                                OrganizationName = record.OrganizationName ?? string.Empty,
                            };

                            var existingRegistration = await _unitOfWork.Repository<Domain.Entities.Registration>()
                                                        .FindAsync(r => r.BatchId == request.CsvData.BatchId &&
                                                                       r.Vin == record.Vin &&
                                                                       r.Grantor == grantor &&
                                                                       r.Spg == spg);

                            if (!existingRegistration.Any()) 
                            {
                                var registration = new Domain.Entities.Registration
                                {
                                    BatchId = request.CsvData.BatchId,
                                    Vin = record.Vin ?? string.Empty,
                                    StartDate = ParseRegistrationDate(record.StartDate ?? string.Empty),
                                    Duration = record.Duration ?? string.Empty,
                                    Grantor = grantor,
                                    Spg = spg
                                };
                                await _unitOfWork.Repository<Domain.Entities.Registration>().AddAsync(registration);
                                addedRecords++;
                            }
                            else
                            {
                                updatedRecords++;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Error processing record with VIN {Vin}", record.Vin);
                            batch.InvalidRecords++;
                            continue; 
                        }
                    }

                    batch.AddedRecords = addedRecords;
                    batch.UpdatedRecords = updatedRecords;
                }

                _unitOfWork.Repository<Domain.Entities.BatchOperation>().Update(batch);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing batch {BatchId}", batch.BatchId);
                throw;
            }

            // Publish events and return result
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
        private static List<Domain.Entities.InvalidRecord> GetDistinctInvalidRecords(IEnumerable<CsvRowDto> dtos, int batchId) =>
       [.. dtos
           .Select(g => new Domain.Entities.InvalidRecord
           {
               BatchId = batchId,
               RecordData = string.Join(", ",
                            g.GetType()
               .GetProperties()
                                  .Where(p => p.Name != "Errors")
                                  .Select(p => p.GetValue(g)?.ToString() ?? string.Empty)),
               ErrorMessages = g.Errors != null ? string.Join(",", g.Errors.Select(n => $"'{n}'")) : string.Empty,
               CreatedAt = DateTime.Now
           })];
        private DateTime ParseRegistrationDate(string dateString)
        {
            if (DateTime.TryParseExact(dateString, "yyyy/MM/dd",
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
            {
                return result;
            }
            throw new FormatException($"Invalid date format: {dateString}. Expected format: yyyy/MM/dd");
        }
    }
}
