using Application.Features.BatchOperation.Commands;
using Application.Features.BatchOperation.Dtos;
using Application.Features.BatchOperation.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/Registration")]
public class RegistrationController : ControllerBase
{
    private readonly IMediator _mediator;

    public RegistrationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("process")]
    public async Task<ActionResult<BatchDto>> ProcessBatch([FromBody] CreateRegistrationsDto Registration)
    {
        // Verify BatchId exists
        if (Registration.Rows == null || Registration.Rows != null && Registration.Rows.Count == 0)
            return BadRequest("Invalid Data");

        // Get Batch Details
        var query = new GetBatchByIdQuery { BatchId = Registration.BatchId };
        var batch = await _mediator.Send(query);

        // Tha ProcessBatchCommand should be an async distributed operation and should be handled by Servicebus or some queue or distributed service
        // Due to time constraint, it's handled as part of this web api
        // Process the batch
        var processBatchCommand = new ProcessBatchCommand
        {
            CsvData = Registration
        };
        var result = await _mediator.Send(processBatchCommand);

        return Ok(batch);
    }
}