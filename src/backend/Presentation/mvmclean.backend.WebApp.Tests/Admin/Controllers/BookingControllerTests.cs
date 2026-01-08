using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using mvmclean.backend.WebApp.Areas.Admin.Controllers;
using mvmclean.backend.Application.Features.Booking.Queries;
using mvmclean.backend.Application.Features.Services.Queries;
using mvmclean.backend.Application.Features.Contractor.Queries;
using mvmclean.backend.Domain.Aggregates.Booking.Enums;

namespace mvmclean.backend.WebApp.Tests.Admin.Controllers;

public class BookingControllerTests_Simplified
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly BookingController _controller;

    public BookingControllerTests_Simplified()
    {
        _mediatorMock = new Mock<IMediator>();
        _controller = new BookingController(_mediatorMock.Object);
    }

    #region Create GET Tests

    [Fact]
    public async Task Create_Get_WithValidRequest_ReturnsViewWithServicesAndContractors()
    {
        // Arrange
        var services = new List<GetAllServicesResponse>
        {
            new GetAllServicesResponse { ServiceId = Guid.NewGuid(), Name = "Carpet Cleaning", Description = "Full carpet cleaning", BasePrice = 89.99m },
            new GetAllServicesResponse { ServiceId = Guid.NewGuid(), Name = "Window Cleaning", Description = "Professional window cleaning", BasePrice = 49.99m }
        };

        var contractors = new List<ContractorListDto>
        {
            new ContractorListDto { Id = Guid.NewGuid(), FullName = "John Doe", IsActive = true },
            new ContractorListDto { Id = Guid.NewGuid(), FullName = "Jane Smith", IsActive = false }
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllServicesRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(services);

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllContractorsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetAllContractorsResponse { Contractors = contractors });

        // Act
        var result = await _controller.Create();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Null(viewResult.Model);
        Assert.NotNull(viewResult.ViewData["Services"]);
        Assert.NotNull(viewResult.ViewData["Contractors"]);
    }

    #endregion

    #region Other Actions Tests

    [Fact]
    public async Task AllBookings_WithValidRequest_ReturnsViewWithBookings()
    {
        // Arrange
        var bookingDtos = new List<BookingDto>
        {
            new BookingDto { Id = Guid.NewGuid(), CustomerName = "John Doe", Status = BookingStatus.Pending, PhoneNumber = "07900123456", Postcode = "SW1A 2AA", TotalPrice = 89.99m },
            new BookingDto { Id = Guid.NewGuid(), CustomerName = "Jane Smith", Status = BookingStatus.Confirmed, PhoneNumber = "07900654321", Postcode = "E1 6AN", TotalPrice = 149.99m }
        };

        var response = new GetAllBookingsResponse { Bookings = bookingDtos };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetAllBookingsRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.AllBookings();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult.Model);
    }

    [Fact]
    public async Task Details_WithValidBookingId_ReturnsBookingDetails()
    {
        // Arrange
        var bookingId = Guid.NewGuid();
        var booking = new GetBookingByIdResponse
        {
            Id = bookingId,
            PhoneNumber = "07900123456",
            Postcode = "SW1A 2AA",
            TotalPrice = 89.99m,
            Currency = "GBP",
            Status = BookingStatus.Pending
        };

        _mediatorMock
            .Setup(m => m.Send(It.IsAny<GetBookingByIdRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(booking);

        // Act
        var result = await _controller.Details(bookingId.ToString());

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.NotNull(viewResult.Model);
    }

    #endregion
}
