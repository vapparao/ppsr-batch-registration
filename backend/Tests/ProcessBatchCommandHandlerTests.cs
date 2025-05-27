using Moq;
using AutoMapper;
using MediatR;
using Domain.Entities;
using Application.Features.BatchOperation.Commands;
using Application.Features.BatchOperation.Dtos;
using Domain.Interfaces;
using System.Linq.Expressions;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;

namespace Tests;

public class ProcessBatchCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<ILogger<ProcessBatchCommandHandler>> _loggerMock;
    private readonly ProcessBatchCommandHandler _handler;


    public ProcessBatchCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();
        _loggerMock = new Mock<ILogger<ProcessBatchCommandHandler>>();
        _handler = new ProcessBatchCommandHandler(_unitOfWorkMock.Object, _mapperMock.Object, _mediatorMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_WhenBatchNotFound_ThrowsArgumentException()
    {
        // Arrange
        var request = new ProcessBatchCommand
        {
            CsvData = new CreateRegistrationsDto { BatchId = 1, Rows = new List<CsvRowDto>() }
        };

        _unitOfWorkMock.Setup(u => u.Repository<BatchOperation>()
                .GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync((BatchOperation?)null);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => _handler.Handle(request, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_WithInvalidRecords_ProcessesInvalidRecords()
    {
        // Arrange
        var batchId = 1;
        var request = new ProcessBatchCommand
        {
            CsvData = new CreateRegistrationsDto
            {
                BatchId = batchId,
                Rows = new List<CsvRowDto>
            {
                new CsvRowDto { IsValid = false, Errors = new List<string> { "Error1" } },
                new CsvRowDto { IsValid = false, Errors = new List<string> { "Error2" } }
            }
            }
        };

        var batch = new BatchOperation { BatchId = batchId };

        _unitOfWorkMock.Setup(u => u.Repository<BatchOperation>().GetByIdAsync(batchId))
            .ReturnsAsync(batch);

        _unitOfWorkMock.Setup(u => u.Repository<InvalidRecord>()
            .AddRangeAsync(It.IsAny<IEnumerable<InvalidRecord>>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.CompleteAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.Repository<InvalidRecord>()
            .AddRangeAsync(It.IsAny<IEnumerable<InvalidRecord>>()), Times.Once);

        Assert.Equal(2, batch.InvalidRecords);
        Assert.Equal(2, batch.TotalRecords);
    }

    [Fact]
    public async Task Handle_WithValidRecords_CreatesNewRegistration()
    {
        // Arrange
        var batchId = 1;
        var request = new ProcessBatchCommand
        {
            CsvData = new CreateRegistrationsDto
            {
                BatchId = batchId,
                Rows = new List<CsvRowDto>
            {
                new CsvRowDto
                {
                    IsValid = true,
                    Vin = "VIN123",
                    FirstName = "John",
                    LastName = "Doe",
                    Acn = "ACN123",
                    OrganizationName = "Org1",
                    StartDate = "2023/01/01",
                    Duration = "1 year"
                }
            }
            }
        };

        var batch = new BatchOperation { BatchId = batchId };

        _unitOfWorkMock.Setup(u => u.Repository<BatchOperation>().GetByIdAsync(batchId))
            .ReturnsAsync(batch);

        _unitOfWorkMock.Setup(u => u.Repository<Grantor>()
            .FindAsync(It.IsAny<Expression<Func<Grantor, bool>>>()))
            .ReturnsAsync(new List<Grantor>());

        _unitOfWorkMock.Setup(u => u.Repository<Spg>()
            .FindAsync(It.IsAny<Expression<Func<Spg, bool>>>()))
            .ReturnsAsync(new List<Spg>());

        _unitOfWorkMock.Setup(u => u.Repository<Registration>()
            .FindAsync(It.IsAny<Expression<Func<Registration, bool>>>()))
            .ReturnsAsync(new List<Registration>());

       _unitOfWorkMock.Setup(u => u.Repository<Registration>()
            .AddAsync(It.IsAny<Registration>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.CompleteAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.Repository<Registration>()
            .AddAsync(It.Is<Registration>(r =>
                r.Vin == "VIN123" &&
                r.BatchId == batchId)),
            Times.Once);

        Assert.Equal(1, batch.AddedRecords);
        Assert.Equal(0, batch.UpdatedRecords);
    }

    [Fact]
    public async Task Handle_WithExistingRegistration_UpdatesCount()
    {
        // Arrange
        var batchId = 1;
        var request = new ProcessBatchCommand
        {
            CsvData = new CreateRegistrationsDto
            {
                BatchId = batchId,
                Rows = new List<CsvRowDto>
                    {
                        new CsvRowDto
                        {
                            IsValid = true,
                            Vin = "VIN123",
                            FirstName = "John",
                            LastName = "Doe",
                            Acn = "ACN123",
                            OrganizationName = "Org1",
                            StartDate = "2023/01/01",
                            Duration = "1 year"
                        }
                    }
            }
        };

        var batch = new BatchOperation { BatchId = batchId };
        var existingRegistration = new Registration { BatchId = batchId, Vin = "VIN123" };

        _unitOfWorkMock.Setup(u => u.Repository<BatchOperation>().GetByIdAsync(batchId))
            .ReturnsAsync(batch);
        _unitOfWorkMock.Setup(u => u.Repository<Grantor>()
           .FindAsync(It.IsAny<Expression<Func<Grantor, bool>>>()))
           .ReturnsAsync(new List<Grantor>());
        _unitOfWorkMock.Setup(u => u.Repository<Spg>()
             .FindAsync(It.IsAny<Expression<Func<Spg, bool>>>()))
             .ReturnsAsync(new List<Spg>());
        _unitOfWorkMock.Setup(u => u.Repository<Registration>()
            .FindAsync(It.IsAny<Expression<Func<Registration, bool>>>()))
            .ReturnsAsync(new List<Registration> { existingRegistration });
        _unitOfWorkMock.Setup(u => u.CompleteAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.Repository<Registration>().AddAsync(It.IsAny<Registration>()), Times.Never);
        Assert.Equal(0, batch.AddedRecords);
        Assert.Equal(1, batch.UpdatedRecords);
    }

    [Fact]
    public async Task Handle_WithMixedValidAndInvalidRecords_ProcessesBoth()
    {
        // Arrange
        var batchId = 1;
        var request = new ProcessBatchCommand
        {
            CsvData = new CreateRegistrationsDto
            {
                BatchId = batchId,
                Rows = new List<CsvRowDto>
            {
                new CsvRowDto { IsValid = false, Errors = new List<string> { "Error1" } },
                new CsvRowDto
                {
                    IsValid = true,
                    Vin = "VIN123",
                    FirstName = "John",
                    LastName = "Doe",
                    Acn = "ACN123",
                    OrganizationName = "Org1",
                    StartDate = "2023/01/01",
                    Duration = "1 year"
                }
            }
            }
        };

        var batch = new BatchOperation { BatchId = batchId };

        _unitOfWorkMock.Setup(u => u.Repository<BatchOperation>().GetByIdAsync(batchId))
            .ReturnsAsync(batch);

        _unitOfWorkMock.Setup(u => u.Repository<InvalidRecord>()
            .AddRangeAsync(It.IsAny<IEnumerable<InvalidRecord>>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.Repository<Grantor>()
            .FindAsync(It.IsAny<Expression<Func<Grantor, bool>>>()))
            .ReturnsAsync(new List<Grantor>());

        _unitOfWorkMock.Setup(u => u.Repository<Spg>()
            .FindAsync(It.IsAny<Expression<Func<Spg, bool>>>()))
            .ReturnsAsync(new List<Spg>());

        _unitOfWorkMock.Setup(u => u.Repository<Registration>()
            .FindAsync(It.IsAny<Expression<Func<Registration, bool>>>()))
            .ReturnsAsync(new List<Registration>());

        _unitOfWorkMock.Setup(u => u.CompleteAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.Repository<InvalidRecord>()
            .AddRangeAsync(It.IsAny<IEnumerable<InvalidRecord>>()), Times.Once);

        _unitOfWorkMock.Verify(u => u.Repository<Registration>()
            .AddAsync(It.IsAny<Registration>()), Times.Once);

        Assert.Equal(1, batch.InvalidRecords);
        Assert.Equal(2, batch.TotalRecords);
        Assert.Equal(1, batch.AddedRecords);
    }

    [Fact]
    public async Task Handle_WithValidDate_ParsesCorrectly()
    {
        // Arrange
        var batchId = 1;
        var request = new ProcessBatchCommand
        {
            CsvData = new CreateRegistrationsDto
            {
                BatchId = batchId,
                Rows = new List<CsvRowDto>
            {
                new CsvRowDto
                {
                    IsValid = true,
                    Vin = "VIN123",
                    FirstName = "John",
                    LastName = "Doe",
                    Acn = "ACN123",
                    OrganizationName = "Org1",
                    StartDate = "2023/01/01",  
                    Duration = "1 year"
                }
            }
            }
        };

        var batch = new BatchOperation { BatchId = batchId };

        _unitOfWorkMock.Setup(u => u.Repository<BatchOperation>().GetByIdAsync(batchId))
            .ReturnsAsync(batch);

        _unitOfWorkMock.Setup(u => u.Repository<Grantor>()
            .FindAsync(It.IsAny<Expression<Func<Grantor, bool>>>()))
            .ReturnsAsync(new List<Grantor>());

        _unitOfWorkMock.Setup(u => u.Repository<Spg>()
            .FindAsync(It.IsAny<Expression<Func<Spg, bool>>>()))
            .ReturnsAsync(new List<Spg>());

        _unitOfWorkMock.Setup(u => u.Repository<Registration>()
            .FindAsync(It.IsAny<Expression<Func<Registration, bool>>>()))
            .ReturnsAsync(new List<Registration>());

        _unitOfWorkMock.Setup(u => u.Repository<Registration>()
            .AddAsync(It.IsAny<Registration>()))
            .Returns(Task.CompletedTask);

        _unitOfWorkMock.Setup(u => u.CompleteAsync())
            .ReturnsAsync(1);

        var result = await _handler.Handle(request, CancellationToken.None);

        // Assert
        _unitOfWorkMock.Verify(u => u.Repository<Registration>()
            .AddAsync(It.Is<Registration>(r =>
                r.StartDate == new DateTime(2023, 1, 1))),
            Times.Once);
    }
    
}
