using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using mvmclean.backend.WebApp.Areas.Api.Controllers;
using mvmclean.backend.Application.Features.Contractor;
using mvmclean.backend.Application.Features.Contractor.Queries;

namespace mvmclean.backend.WebApp.Tests.Api.Controllers;

public class ContractorControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly ContractorController _controller;

    public ContractorControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new ContractorController(_mediatorMock.Object);
    }

    [Fact]
    public async Task GetContractorsByPostcode_WithValidPostcode_ReturnsOkResult()
    {
        // Arrange
        var postcode = "SW1A1AA";
        var contractorIds = new List<string> 
        { 
            "contractor-1", 
            "contractor-2", 
            "contractor-3" 
        };
        var response = new GetContractorsByPostcodeResponse
        {
            ContractorIds = contractorIds,
            BookingId = ""
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetContractorsByPostcodeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetContractorsByPostcode(postcode);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        _mediatorMock.Verify(
            m => m.Send(It.IsAny<GetContractorsByPostcodeRequest>(), It.IsAny<CancellationToken>()), 
            Times.Once);
    }

    [Fact]
    public async Task GetContractorsByPostcode_WithEmptyPostcode_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.GetContractorsByPostcode("");

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact]
    public async Task GetContractorsByPostcode_WithNullPostcode_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.GetContractorsByPostcode(null);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, objectResult.StatusCode);
    }

    [Fact]
    public async Task GetContractorsByPostcode_WithNoResults_ReturnsOkWithEmptyList()
    {
        // Arrange
        var postcode = "XX99XX";
        var response = new GetContractorsByPostcodeResponse
        {
            ContractorIds = new List<string>(),
            BookingId = ""
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetContractorsByPostcodeRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetContractorsByPostcode(postcode);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task GetContractorsByPostcode_WhenMediatorThrows_ReturnsInternalServerError()
    {
        // Arrange
        var postcode = "SW1A1AA";
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetContractorsByPostcodeRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _controller.GetContractorsByPostcode(postcode);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task GetContractorsAvailabilityByDay_WithValidRequest_ReturnsOkResult()
    {
        // Arrange
        var futureDate = DateTime.Now.AddDays(1);
        var request = new GetContractorAvailabilityByDayRequest
        {
            ContractorIds = new List<string> { "contractor-1" },
            Date = futureDate,
            Duration = TimeSpan.FromMinutes(60)
        };
        var response = new List<GetContractorAvailabilityByDayResponse>
        {
            new GetContractorAvailabilityByDayResponse
            {
                ContractorId = "contractor-1",
                StartTime = futureDate.AddHours(9).ToString("yyyy-MM-dd HH:mm"),
                EndTime = futureDate.AddHours(10).ToString("yyyy-MM-dd HH:mm"),
                Available = true
            }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetContractorAvailabilityByDayRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetContractorsAvailabilityByDay(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task GetContractorsAvailabilityByDay_WithNoAvailableSlots_ReturnsOkWithEmptyList()
    {
        // Arrange
        var futureDate = DateTime.Now.AddDays(1);
        var request = new GetContractorAvailabilityByDayRequest
        {
            ContractorIds = new List<string> { "contractor-1" },
            Date = futureDate,
            Duration = TimeSpan.FromMinutes(60)
        };
        var response = new List<GetContractorAvailabilityByDayResponse>();

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetContractorAvailabilityByDayRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetContractorsAvailabilityByDay(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task GetContractorsAvailabilityByDay_WhenMediatorThrows_ReturnsInternalServerError()
    {
        // Arrange
        var request = new GetContractorAvailabilityByDayRequest
        {
            ContractorIds = new List<string> { "contractor-1" },
            Date = DateTime.Now.AddDays(1),
            Duration = TimeSpan.FromMinutes(60)
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetContractorAvailabilityByDayRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetContractorsAvailabilityByDay(request);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }
}
