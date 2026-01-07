using MediatR;
using Microsoft.AspNetCore.Mvc;
using mvmclean.backend.WebApp.Areas.Api.Models;

namespace mvmclean.backend.WebApp.Areas.Api.Controllers;

[Route("api/[controller]")]
public class BasketController : BaseApiController
{
    // In-memory basket storage (replace with Redis or database in production)
    private static readonly Dictionary<string, BasketModel> _baskets = new();

    public BasketController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Gets basket by session ID
    /// </summary>
    [HttpGet("{sessionId}")]
    public IActionResult GetBasket(string sessionId)
    {
        if (_baskets.TryGetValue(sessionId, out var basket))
        {
            return Success(basket);
        }

        return Success(new BasketModel { SessionId = sessionId, Items = new List<BasketItem>() });
    }

    /// <summary>
    /// Adds service to basket
    /// </summary>
    [HttpPost("add")]
    public IActionResult AddToBasket([FromBody] AddToBasketRequest request)
    {
        if (!_baskets.ContainsKey(request.SessionId))
        {
            _baskets[request.SessionId] = new BasketModel
            {
                SessionId = request.SessionId,
                Items = new List<BasketItem>()
            };
        }

        var basket = _baskets[request.SessionId];

        // Check if service already exists
        var existingItem = basket.Items.FirstOrDefault(i => i.ServiceId == request.ServiceId);
        if (existingItem != null)
        {
            existingItem.Quantity++;
        }
        else
        {
            basket.Items.Add(new BasketItem
            {
                ServiceId = request.ServiceId,
                ServiceName = request.ServiceName,
                Price = request.Price,
                Duration = request.Duration,
                Quantity = 1
            });
        }

        basket.UpdateTotal();
        return Success(basket, "Service added to basket");
    }

    /// <summary>
    /// Removes service from basket
    /// </summary>
    [HttpPost("remove")]
    public IActionResult RemoveFromBasket([FromBody] RemoveFromBasketRequest request)
    {
        if (!_baskets.TryGetValue(request.SessionId, out var basket))
        {
            return Error("Basket not found", 404);
        }

        var item = basket.Items.FirstOrDefault(i => i.ServiceId == request.ServiceId);
        if (item != null)
        {
            basket.Items.Remove(item);
            basket.UpdateTotal();
        }

        return Success(basket, "Service removed from basket");
    }

    /// <summary>
    /// Updates service quantity in basket
    /// </summary>
    [HttpPost("update-quantity")]
    public IActionResult UpdateQuantity([FromBody] UpdateQuantityRequest request)
    {
        if (!_baskets.TryGetValue(request.SessionId, out var basket))
        {
            return Error("Basket not found", 404);
        }

        var item = basket.Items.FirstOrDefault(i => i.ServiceId == request.ServiceId);
        if (item != null)
        {
            item.Quantity = request.Quantity;
            if (item.Quantity <= 0)
            {
                basket.Items.Remove(item);
            }
            basket.UpdateTotal();
        }

        return Success(basket, "Quantity updated");
    }

    /// <summary>
    /// Clears basket
    /// </summary>
    [HttpPost("clear/{sessionId}")]
    public IActionResult ClearBasket(string sessionId)
    {
        if (_baskets.ContainsKey(sessionId))
        {
            _baskets.Remove(sessionId);
        }

        return Success(new BasketModel { SessionId = sessionId, Items = new List<BasketItem>() }, "Basket cleared");
    }
}
