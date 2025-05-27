using Application.Features.BatchOperation.Commands;
using Application.Features.BatchOperation.Dtos;
using Application.Features.BatchOperation.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/batches")]
public class BatchController : ControllerBase
{
    private readonly IMediator _mediator;

    public BatchController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("upload")]
    public async Task<ActionResult<BatchDto>> UploadBatch([FromBody] CreateBatchDto batch)
    {
        // Verify file name exists
        if (batch.FileName == null)
            return BadRequest("Invalid Filename");
        // Verify file checksum exists
        if (batch.FileChecksum == null)
            return BadRequest("Invalid Checksum");

        // Check if batch already exists
        var query = new GetBatchByChecksumQuery { FileChecksum = batch.FileChecksum, ClientId = batch.ClientId };
        var existingBatch = await _mediator.Send(query);
        if (existingBatch == null)
        {
            // Create the batch
            var command = new CreateBatchCommand
            {
                Batch = batch
            };

            var batchResult = await _mediator.Send(command);
            return Ok(batchResult);
        }

        return Ok(existingBatch);
    }
}