using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using mvmclean.backend.WebApp.Areas.Api.Controllers;
using mvmclean.backend.Application.Features.Booking.Commands;
using mvmclean.backend.Application.Features.Booking.Queries;
using mvmclean.backend.Application.Services;

namespace mvmclean.backend.WebApp.Tests.Api.Controllers;

public class BookingControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IStripeService> _stripeServiceMock;
    private readonly BookingController _controller;

    public BookingControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _stripeServiceMock = new Mock<IStripeService>();
        _controller = new BookingController(_mediatorMock.Object, _stripeServiceMock.Object);
    }

    [Fact]
    public async Task CreateBooking_WithValidRequest_ReturnsOkResult()
    {
        // Arrange
        var request = new CreateBookingRequest
        {
            PhoneNumber = "07123456789",
            Postcode = "SW1A1AA"
        };
        var response = new CreateBookingResponse 
        { 
            BookingId = "123e4567-e89b-12d3-a456-426614174000" 
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateBookingRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.CreateBooking(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
        _mediatorMock.Verify(m => m.Send(It.IsAny<CreateBookingRequest>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateBooking_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateBookingRequest();
        _controller.ModelState.AddModelError("PhoneNumber", "Phone number is required");

        // Act
        var result = await _controller.CreateBooking(request);

        // Assert
        var badResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, badResult.StatusCode);
    }

    [Fact]
    public async Task CreateBooking_WhenMediatorThrows_ReturnsInternalServerError()
    {
        // Arrange
        var request = new CreateBookingRequest
        {
            PhoneNumber = "07123456789",
            Postcode = "SW1A1AA"
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<CreateBookingRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateBooking(request);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }

    [Fact]
    public async Task GetBookingById_WithValidGuid_ReturnsOkResult()
    {
        // Arrange
        var bookingId = Guid.NewGuid().ToString();
        var response = new GetBookingByIdResponse
        {
            Id = Guid.Parse(bookingId),
            PhoneNumber = "07123456789",
            Postcode = "SW1A1AA",
            Currency = "GBP"
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetBookingByIdRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetBookingById(bookingId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(200, okResult.StatusCode);
    }

    [Fact]
    public async Task GetBookingById_WithEmptyId_ReturnsBadRequest()
    {
        // Act
        var result = await _controller.GetBookingById("");

        // Assert
        var badResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, badResult.StatusCode);
    }

    [Fact]
    public async Task GetBookingById_WithNullId_ReturnsBadRequest()
    {
        // Act
#pragma warning disable CS8625
        var result = await _controller.GetBookingById(null);
#pragma warning restore CS8625

        // Assert
        var badResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, badResult.StatusCode);
    }

    [Fact]
    public async Task GetBookingById_WithInvalidGuidFormat_ReturnsBadRequest()
    {
        // Arrange
        var invalidId = "not-a-valid-guid";

        // Act
        var result = await _controller.GetBookingById(invalidId);

        // Assert
        var badResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(400, badResult.StatusCode);
    }

    [Fact]
    public async Task GetBookingById_WhenMediatorThrows_ReturnsInternalServerError()
    {
        // Arrange
        var bookingId = Guid.NewGuid().ToString();
        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetBookingByIdRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetBookingById(bookingId);

        // Assert
        var objectResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, objectResult.StatusCode);
    }
}
